# Eventuous aggregate-less bootstrap experiment

## Goal

`POST profiles/bootstrap-state` is a side-by-side experiment for bootstrapping a profile without loading Eventuous aggregates.

The production `POST profiles/bootstrap` endpoint remains unchanged and still uses:

- `UniqueExternalIdentityAggregate`
- `ProfileAggregate`
- aggregate methods that validate and apply new events

The experiment keeps the same HTTP request and response contracts, but the command handler loads folded state directly from Eventuous.

## Aggregate version

The existing bootstrap handler loads two aggregates:

1. `UniqueExternalIdentityAggregate` from the external identity stream.
2. `ProfileAggregate` from the generated profile stream.

Domain methods then decide and stage changes:

- `UniqueExternalIdentityAggregate.Claim(profileId)`
- `ProfileAggregate.Bootstrap(...)`

When both aggregates have staged changes, Slicent stores both streams with their original versions.

## Aggregate-less version

The aggregate-less handler loads two state objects:

1. `UniqueExternalIdentityState` from the external identity stream.
2. `ProfileState` from the generated profile stream.

Eventuous folds each stream into state with `LoadState`. The handler then decides which events to append:

- `UniqueExternalIdDomainEvents.V1.Claimed`
- `ProfileDomainEvents.V1.Created`
- `ProfileDomainEvents.V1.ExternalIdAssociated`
- `ProfileDomainEvents.V1.EmailVerified`, when the provider confirms the email

The important difference is that the handler owns the decision logic. There is no aggregate method collecting changes.

## Concurrency

Aggregate-less does not remove optimistic concurrency requirements.

Bootstrap still writes two streams:

- the profile stream
- the unique external identity stream

The aggregate-less helper stores each stream with the expected version returned by `LoadState`. If another request claims the same external identity between load and append, Eventuous raises `OptimisticConcurrencyException`, and the endpoint returns the same conflict response shape as the aggregate implementation.

## Slicent support

Slicent adds only small helper methods for this experiment:

- load a folded state or an empty state
- convert folded state plus new events into stream changes
- store non-empty stream changes with expected versions

This keeps the experiment close to the current vertical slice style and avoids adopting Eventuous functional command services before the tradeoffs are clearer.
