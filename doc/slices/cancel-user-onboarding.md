# Cancel user onboarding

## Purpose

Cancel an in-progress user onboarding process.

Cancellation records a terminal onboarding fact and releases the claimed external identity and optional alias so they can be reused by a later onboarding process.

Completed onboarding cannot be cancelled because the completed process has already created a user.

## Endpoint

`POST /user-onboarding/{id}/cancel`

The route id is the user onboarding id. The authenticated principal must contain the matching app-owned `user_onboarding_id` claim.

## Inputs

The request body contains:

- `reason`, required

## Streams and events

When the onboarding has an alias, cancellation writes three process streams atomically.

The user onboarding stream receives:

- `UserOnboardingCanceled`

The external identity claiming stream receives:

- `ExternalIdentityReleased`

The alias claiming stream receives:

- `AliasReleased`

When no alias has been chosen, only the user onboarding stream and external identity claiming stream are written.

## Idempotency

The public HTTP endpoint is not idempotent after a successful cancellation.

Once cancellation releases the external identity claim, later requests with the same external identity no longer receive a `user_onboarding_id` claim and are rejected by authorization.

The command handler still treats an already-cancelled stream as idempotent defensive application behavior if invoked directly.

## Consistency

Cancellation must cancel the onboarding and release all owned claims as one business decision.

The handler loads the onboarding stream, the external identity claiming stream, and the optional alias claiming stream. It verifies each claim is still owned by the onboarding being cancelled, then appends all resulting events with expected stream versions in one multi-stream write.

If any stream changes between load and append, Eventuous raises an optimistic concurrency exception and the slice returns a conflict result.

## Decisions

- Claiming streams represent current availability, not historical existence. A stream with a claim followed by a release is available for reuse.
- Cancellation is a terminal state for onboarding. Public onboarding endpoints require a current `user_onboarding_id` claim after cancellation, so cancelled onboarding is no longer reachable through those endpoints once claims are released.
- Cancellation reason is stored on the onboarding process stream. Release events only record the owning user onboarding id.
- Retrieve user onboarding requires a current `user_onboarding_id` claim, so cancelled onboarding is no longer retrievable through the public onboarding route after its external identity claim is released.
