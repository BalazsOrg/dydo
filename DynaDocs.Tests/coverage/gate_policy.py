#!/usr/bin/env python3
"""Validate coverage measurements and enforce the retained DR068 callable statics."""

def _nonnegative_integer(value):
    if type(value) is not int or value < 0:
        raise ValueError(f"Expected nonnegative integer, received {value!r}")
    return value


def _validate_hits(hits):
    if not isinstance(hits, dict):
        raise ValueError("Coverage points must be an identity-to-hit mapping")
    for hit in hits.values():
        _nonnegative_integer(hit)


def _violation(module, gate, actual, threshold, member=None):
    return {"path": module["path"], "gate": gate, "actual": float(actual),
            "threshold": threshold, "member": member}


def _method_violations(module, method):
    try:
        _nonnegative_integer(method["cc"])
        cognitive, parameters, covered, total = (
            _nonnegative_integer(method[key])
            for key in ("cognitive", "parameters", "covered", "total"))
        identity = method["id"]
        constructor = method["constructor"]
    except KeyError as error:
        raise ValueError(f"Missing method fact: {error}") from error
    if not identity or type(constructor) is not bool or total == 0 or covered > total:
        raise ValueError(f"Invalid executable method facts: {method!r}")
    metrics = [("cognitive", cognitive, 20)]
    if not constructor:
        metrics.append(("parameters", parameters, 7))
    return [_violation(module, gate, actual, limit, identity)
            for gate, actual, limit in metrics if actual > limit]


def _module_violations(module):
    try:
        _validate_hits(module["lines"])
        _validate_hits(module["branches"])
        methods = module["methods"]
        executable = module["executable"]
    except KeyError as error:
        raise ValueError(f"Missing module fact: {error}") from error
    if type(executable) is not bool or not isinstance(methods, list):
        raise ValueError("Invalid executable flag or method list")
    if not executable:
        if methods or module["lines"] or module["branches"]:
            raise ValueError("Declarative module contains executable facts")
        return []
    if (not methods and not module.get('body_owners')) or not module["lines"]:
        raise ValueError(f"Executable module has no methods/points: {module['path']}")
    identities = [method.get("id") for method in methods]
    if len(set(identities)) != len(identities):
        raise ValueError("Duplicate method identity")
    findings = []
    for method in methods:
        findings.extend(_method_violations(module, method))
    return findings


def _body_owner_points(module, metrics, logical_points):
    owners = module['body_owners']
    if not isinstance(owners, list):
        raise ValueError('Invalid emitted body ownership inventory')
    lines, branches, physical = {}, {}, set()
    for owner in owners:
        if set(owner) != {'physical', 'key', 'logical', 'structural', 'lines', 'branches'}:
            raise ValueError('Incomplete emitted body ownership proof')
        identity = owner['physical']
        if not identity or not owner['key'] or identity in physical or not owner['lines']:
            raise ValueError('Missing or duplicate emitted body identity')
        physical.add(identity)
        _validate_hits(owner['lines'])
        _validate_hits(owner['branches'])
        for line, hits in owner['lines'].items():
            lines[line] = max(lines.get(line, 0), hits)
        if set(branches) & set(owner['branches']):
            raise ValueError('Duplicate emitted body branch identity')
        branches.update(owner['branches'])
        _body_destination(module, owner, metrics, logical_points)
    if lines != module['lines'] or branches != module['branches']:
        raise ValueError('Emitted body ownership does not account for every module point')


def _body_destination(module, owner, metrics, logical_points):
    if owner['structural'] is not None:
        allowed = {'semantic synthesized member without authored body',
                   'semantic synthesized record copy constructor',
                   'semantic implicit constructor with no authored executable fragments'}
        if owner['structural'] not in allowed or owner['logical'] is not None:
            raise ValueError('Unexplained structural emitted body')
        return
    logical = owner['logical']
    if not isinstance(logical, dict) or set(logical) != {'path', 'id'}:
        raise ValueError('Missing logical method destination')
    key = logical['path'], logical['id']
    if key not in metrics or not logical['id'].endswith('|' + owner['physical']):
        raise ValueError('Emitted body has no exact logical method metric')
    points = logical_points.setdefault(key, {})
    for line, hits in owner['lines'].items():
        pair = module['path'], line
        if pair in points:
            raise ValueError('Duplicate logical method body point')
        points[pair] = hits


def _validate_body_owners(modules):
    metrics = {(module['path'], method.get('id')): method for module in modules for method in module.get('methods', [])}
    logical_points = {}
    for module in modules:
        if 'body_owners' in module:
            _body_owner_points(module, metrics, logical_points)
    for key, points in logical_points.items():
        metric = metrics[key]
        if metric.get('total') != len(points) or metric.get('covered') != sum(value > 0 for value in points.values()):
            raise ValueError('Logical method metric and emitted body coverage disagree')


def evaluate_policy(modules):
    """Evaluate complete per-stack facts; missing facts are measurement errors."""
    paths = [module.get("path") for module in modules]
    if not all(paths) or len(set(paths)) != len(paths):
        raise ValueError("Missing or duplicate module identity")
    _validate_body_owners(modules)
    return [finding for module in modules for finding in _module_violations(module)]
