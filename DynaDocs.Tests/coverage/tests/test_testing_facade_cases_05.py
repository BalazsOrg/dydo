"""Inherited facade cases; collected through the two public facade classes."""
import os
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path
from test_testing_facade_support import (has_worktree_identity, ROOT, SCRIPT_PATH)


class Cases:
    """Only the public facade classes collect these inherited unittest cases."""

    class FacadeCases05(unittest.TestCase):
        def test_interruption_restores_all_handlers_before_delivery(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            harness = r'''
import importlib.util
import itertools
import json
import signal
import sys
from types import SimpleNamespace

spec = importlib.util.spec_from_file_location('adapter', sys.argv[1])
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)
native = [signal.SIGINT, *([signal.SIGBREAK] if sys.platform == 'win32' else [])]
original_signal = signal.signal
original_handlers = {sig: signal.getsignal(sig) for sig in native}
observations = []

def prior_int(signum, frame):
    signal.default_int_handler(signum, frame)

def prior_break(signum, frame):
    signal.default_int_handler(signum, frame)

try:
    for platform in dict.fromkeys([sys.platform, 'linux']):
        module.sys = SimpleNamespace(platform=platform)
        installed = native if platform == 'win32' else [signal.SIGINT]
        prior = {sig: prior_int if sig == signal.SIGINT else prior_break for sig in native}
        bodies = [[], *[[sig] for sig in installed], installed * 2, ['KeyboardInterrupt'], ['ValueError']]
        for body, position, phase, injection in itertools.product(
                bodies, installed, ['after', 'before'], [*installed, 'KeyboardInterrupt', None]):
            for sig, handler in prior.items():
                original_signal(sig, handler)
            remaining = 2
            restoration_calls = []

            def restoring_signal(signum, handler):
                global remaining
                restoring = handler is prior[signum]
                if restoring:
                    restoration_calls.append(signal.Signals(signum).name)
                inject = restoring and signum == position and remaining > 0 and injection is not None
                if inject:
                    remaining -= 1
                if phase == 'after' or not inject:
                    result = original_signal(signum, handler)
                if inject:
                    if injection == 'KeyboardInterrupt':
                        raise KeyboardInterrupt
                    signal.raise_signal(injection)
                if phase == 'before' and inject:
                    result = original_signal(signum, handler)
                return result

            signal.signal = restoring_signal
            caught = None
            restored_at_delivery = None
            try:
                with module.defer_interruption():
                    for pending in body:
                        if pending == 'KeyboardInterrupt':
                            raise KeyboardInterrupt
                        if pending == 'ValueError':
                            raise ValueError('body exception')
                        signal.raise_signal(pending)
            except (KeyboardInterrupt, ValueError) as exc:
                caught = type(exc).__name__
                restored_at_delivery = all(signal.getsignal(sig) is handler for sig, handler in prior.items())
            finally:
                signal.signal = original_signal
            restored = all(signal.getsignal(sig) is handler for sig, handler in prior.items())
            subsequent = []
            for sig in installed:
                try:
                    signal.raise_signal(sig)
                except KeyboardInterrupt:
                    subsequent.append(signal.Signals(sig).name)
            row = dict(platform=platform, body=[str(item) for item in body],
                       position=signal.Signals(position).name, phase=phase, injection=str(injection),
                       caught=caught, restored_at_delivery=restored_at_delivery,
                       restored=restored, subsequent=subsequent, restoration_calls=restoration_calls)
            observations.append(row)
            print(json.dumps(row), flush=True)
            expected = 'KeyboardInterrupt' if injection is not None or body else None
            if injection is None and body == ['ValueError']:
                expected = 'ValueError'
            assert caught == expected, row
            assert restored, row
            assert restored_at_delivery if caught else restored_at_delivery is None, row
            assert subsequent == [signal.Signals(sig).name for sig in installed], row
finally:
    signal.signal = original_signal
    for sig, handler in original_handlers.items():
        original_signal(sig, handler)
print('RESTORATION_MATRIX_CASES=' + str(len(observations)))
'''
            process = subprocess.run([sys.executable, '-u', '-c', harness, str(adapter)], cwd=ROOT,
                                     capture_output=True, text=True, encoding='utf-8', timeout=30)
            output = process.stdout + process.stderr
            self.assertEqual(0, process.returncode, output)
            self.assertIn('RESTORATION_MATRIX_CASES=126' if os.name == 'nt' else 'RESTORATION_MATRIX_CASES=30', output)


        def test_interrupt_between_handler_restoration_iterations(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            harness = r'''
import importlib.util
import inspect
import itertools
import json
import signal
import sys
from types import SimpleNamespace

spec = importlib.util.spec_from_file_location('adapter', sys.argv[1])
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)
function = module.defer_interruption.__wrapped__
lines, first = inspect.getsourcelines(function)
loop_line = first + next(i for i, line in enumerate(lines) if 'for signum, handler in previous.items():' in line)
native = [signal.SIGINT, *([signal.SIGBREAK] if sys.platform == 'win32' else [])]
original = {sig: signal.getsignal(sig) for sig in native}
prior = {sig: (lambda signum, frame: signal.default_int_handler(signum, frame)) for sig in native}
count = 0
try:
    for platform, body, injection in itertools.product(
            dict.fromkeys([sys.platform, 'linux']), [None, 'signal', 'ValueError'], ['signal', 'KeyboardInterrupt', None]):
        module.sys = SimpleNamespace(platform=platform)
        for sig, handler in prior.items():
            signal.signal(sig, handler)
        injected = False
        def trace(frame, event, arg):
            global injected
            # The system-call wrapper cannot observe this loop backedge.
            if (frame.f_code is function.__code__ and event == 'line' and frame.f_lineno == loop_line
                    and signal.getsignal(signal.SIGINT) is prior[signal.SIGINT]
                    and injection is not None and not injected):
                injected = True
                if injection == 'KeyboardInterrupt':
                    raise KeyboardInterrupt
                signal.raise_signal(signal.SIGINT)
            return trace
        caught = None
        restored_at_delivery = None
        try:
            with module.defer_interruption():
                sys.settrace(trace)
                if body == 'signal':
                    signal.raise_signal(signal.SIGINT)
                elif body == 'ValueError':
                    raise ValueError('body exception')
        except (KeyboardInterrupt, ValueError) as exc:
            caught = type(exc).__name__
            restored_at_delivery = all(signal.getsignal(sig) is handler for sig, handler in prior.items())
        finally:
            sys.settrace(None)
        restored = all(signal.getsignal(sig) is handler for sig, handler in prior.items())
        subsequent = []
        for sig in native:
            try:
                signal.raise_signal(sig)
            except KeyboardInterrupt:
                subsequent.append(sig)
        row = dict(platform=platform, body=body, injection=injection, injected=injected,
                   caught=caught, restored=restored, restored_at_delivery=restored_at_delivery,
                   subsequent=subsequent)
        print(json.dumps(row), flush=True)
        expected = 'KeyboardInterrupt' if injection is not None or body == 'signal' else body
        assert injected == (injection is not None), row
        assert caught == expected, row
        assert restored and (restored_at_delivery if caught else restored_at_delivery is None), row
        assert subsequent == native, row
        count += 1
finally:
    sys.settrace(None)
    for sig, handler in original.items():
        signal.signal(sig, handler)
print('INTER_ITERATION_CASES=' + str(count))
'''
            process = subprocess.run([sys.executable, '-u', '-c', harness, str(adapter)], cwd=ROOT,
                                     capture_output=True, text=True, encoding='utf-8', timeout=30)
            output = process.stdout + process.stderr
            self.assertEqual(0, process.returncode, output)
            self.assertIn('INTER_ITERATION_CASES=18' if os.name == 'nt' else 'INTER_ITERATION_CASES=9', output)


        def test_exact_worktree_registration_and_failed_removal_cleanup(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            for failed_remove in [False, True]:
                with self.subTest(failed_remove=failed_remove), tempfile.TemporaryDirectory(prefix='dydo-registration-audit-') as temporary:
                    candidate = Path(temporary) / 'owned-\u00e1-path'
                    harness = (
                        "import importlib.util\nfrom pathlib import Path\nfrom types import SimpleNamespace\n"
                        f"spec=importlib.util.spec_from_file_location('adapter', {str(adapter)!r})\n"
                        "module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)\n"
                        f"path=Path({str(candidate)!r}); path.mkdir()\n"
                        "assert module.create_worktree(path)\n"
                        "original=module._git\n"
                        "try:\n"
                        "    assert module.is_registered_worktree(path), 'exact non-ASCII registration not recognized'\n"
                        "    if module.sys.platform == 'win32': assert module.is_registered_worktree(Path(str(path).upper()))\n"
                        "    assert not module.is_registered_worktree(Path(str(path)+'-sibling'))\n"
                        f"    if {failed_remove!r}:\n"
                        "        def fail_first_remove(*args, capture=False):\n"
                        "            if args[:3] == ('worktree', 'remove', '--force'):\n"
                        "                module._git=original\n"
                        "                print('FIRST_GIT_REMOVE_FAILED', flush=True)\n"
                        "                return SimpleNamespace(returncode=1)\n"
                        "            return original(*args, capture=capture)\n"
                        "        module._git=fail_first_remove\n"
                        "    module.remove_worktree(path)\n"
                        "    assert not path.exists(), 'owned directory survived cleanup'\n"
                        "    assert not module.is_registered_worktree(path), 'owned registration survived fallback cleanup'\n"
                        "finally:\n"
                        "    original('worktree', 'remove', '--force', str(path))\n"
                    )
                    process = subprocess.run([sys.executable, '-u', '-c', harness], cwd=ROOT,
                                             capture_output=True, text=True, encoding='utf-8', errors='replace', timeout=30)
                    self.assertEqual(0, process.returncode, process.stdout + process.stderr)
                    self.assertFalse(candidate.exists())


        def test_failed_git_add_cleans_only_the_acquired_empty_directory(self):
            adapter = ROOT / 'DynaDocs.Tests/coverage/run_tests.py'
            with tempfile.TemporaryDirectory(prefix='dydo-allocation-failure-') as temporary:
                candidate = Path(temporary) / 'dydo-test-fa11ed00'
                harness = (
                    "import importlib.util\nfrom pathlib import Path\nfrom types import SimpleNamespace\n"
                    + SCRIPT_PATH + f"spec=importlib.util.spec_from_file_location('adapter', {str(adapter)!r})\n"
                    "module=importlib.util.module_from_spec(spec); spec.loader.exec_module(module)\n"
                    f"module.tempfile.gettempdir=lambda: {temporary!r}\n"
                    "module.uuid.uuid4=lambda: SimpleNamespace(hex='fa11ed00')\n"
                    "original=module._git\n"
                    "def failed_git(*args, capture=False):\n"
                    "    if args[:3] == ('worktree', 'add', '--detach'):\n"
                    "        path=Path(args[3])\n"
                    "        assert path.is_dir() and not list(path.iterdir())\n"
                    "        print('OWNED_EMPTY_DIRECTORY', flush=True)\n"
                    "        return SimpleNamespace(returncode=1)\n"
                    "    return original(*args, capture=capture)\n"
                    "module._git=failed_git\n"
                    "module.main()\n"
                )
                process = subprocess.run([sys.executable, '-u', '-c', harness], cwd=ROOT,
                                         capture_output=True, text=True, encoding='utf-8', timeout=30)
                output = process.stdout + process.stderr
                self.assertEqual(1, process.returncode, output)
                self.assertIn('OWNED_EMPTY_DIRECTORY', output)
                self.assertNotIn('Traceback', output)
                self.assertFalse(candidate.exists(), output)
                listing = subprocess.run(['git', 'worktree', 'list', '--porcelain'], cwd=ROOT,
                                         text=True, capture_output=True, check=True).stdout
                self.assertFalse(has_worktree_identity(listing, candidate))
