# Slice decision records

Per-slice docs are useful when they capture behavior or decisions that are not obvious from code.

They are not useful when they repeat filenames, class names, or implementation details that the code already expresses.

## When to add one

Add a slice decision record when a slice has:

- non-obvious business rules
- important idempotency behavior
- multi-stream consistency decisions
- auth or identity assumptions
- external integration assumptions
- rejected alternatives that future agents may reintroduce accidentally
- open questions or explicit product assumptions

Do not add one only because a slice exists.

## Suggested location

Use:

```text
doc/slices/<slice-name>.md
```

Examples:

```text
doc/slices/initiate-user-onboarding.md
doc/slices/choose-user-alias.md
doc/slices/complete-user-onboarding.md
```

## Suggested format

```markdown
# Initiate user onboarding

## Purpose

What user or business outcome this slice owns.

## Inputs

Important request, route, auth, or identity inputs.

## Business rules

Rules the handler must enforce.

## Events

Streams and events written by the slice.

## Idempotency

What happens if the command is repeated.

## Consistency

Which streams are written atomically and why.

## Decisions

Implementation decisions that should survive refactoring.

## Open questions

Known gaps that should not be guessed by agents.
```

## Keep it current

Update the slice doc when behavior changes.

Do not update it for mechanical refactors that do not change behavior or implementation decisions.
