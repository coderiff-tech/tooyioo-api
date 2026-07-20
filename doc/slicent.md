# Slicent

Slicent is the local framework for Tooyioo vertical slice conventions.

It sits on top of Eventuous and ASP.NET Core conventions. It should not hide Eventuous or become a business framework.

## What belongs in Slicent

Add code to Slicent when it is:

- generic across modules
- about vertical slice registration or dispatch
- about command/query conventions
- about Eventuous integration
- independent from Tooyioo business concepts

Examples:

- command handler registration
- query handler registration
- endpoint mapping conventions
- event store helper methods
- framework-level testing helpers for Slicent itself

## What does not belong in Slicent

Do not add Tooyioo domain behavior to Slicent.

Avoid putting these in Slicent:

- user onboarding rules
- campaign rules
- merchant rules
- Tooyioo claim names
- module-specific event names
- module-specific read models

## Eventuous relationship

Slicent intentionally does not abstract Eventuous away.

Prefer exposing or composing Eventuous primitives when that keeps behavior clear:

- `IEventReader`
- `IEventWriter`
- folded state
- stream names
- expected stream versions

Only add wrapper APIs when they remove repeated ceremony without hiding important event-sourcing semantics.

## Testing Slicent

Slicent tests belong in `test/Slicent.Tests`.

They should verify framework behavior without depending on Tooyioo application modules. If a test needs Tooyioo domain types to explain the behavior, it probably belongs in `test/Tooyioo.Tests` instead.
