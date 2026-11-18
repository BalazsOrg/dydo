"""Print native unittest IDs, preserving the public base and portable classes."""
from pathlib import Path
import sys
import unittest

root = Path(__file__).resolve().parents[7]
sys.path.insert(0, str(root / 'DynaDocs.Tests/coverage/tests'))
suite = unittest.defaultTestLoader.discover(str(root / 'DynaDocs.Tests/coverage/tests'),
                                          pattern='test_testing_facade*.py')


def ids(tests):
    for case in tests:
        if isinstance(case, unittest.TestSuite):
            yield from ids(case)
        else:
            yield case.id()


names = sorted(ids(suite))
assert names and len(names) == len(set(names))
assert all(name.startswith(('test_testing_facade.TestingFacadeTests.',
                            'test_testing_facade.PortableTestingFacadeTests.')) for name in names)
print('\n'.join(names))
print('COUNT:', len(names))
