# Tooyioo - AI Agent Context

## Overview
This repository contains `tooyioo-api`, the backend for Tooyioo.

Tooyioo is a local demand-generation platform for small and medium businesses. It helps bars, restaurants, shops, and local merchants create campaigns and offers that can be distributed to nearby users, including through street promoters.

A typical use case is a hospitality business running a local promotion. Promoters approach potential customers, offer them a specific deal, and provide a finite numbered QR code that can be redeemed at the venue. This allows:

- merchants to attract demand when they need it
- promoters to keep operating in the real world while tracking attribution
- users to discover better local products, prices, and experiences
- businesses to measure which campaigns, offers, promoters, and audiences work best

The business model is subscription-based for merchants, with free usage options intended to increase market adoption.

## Repository Structure
The main repository folders are:

- `src/` - application and framework source code
- `test/` - automated tests
- `doc/` - documentation, specifications, diagrams, and design notes

Main projects under `src/`:

- `Tooyioo.AppHost` - local development entry point using .NET Aspire
- `Tooyioo.Api` - main ASP.NET Core/Kestrel application, infrastructure, dependency wiring, and `Program.cs`
- `Tooyioo.Common` - Tooyioo-specific shared code that is not generic framework code
- `slicent/Slicent` - reusable framework code built around Eventuous, vertical slices, CQRS, and event sourcing

## Architecture
This repository is a modular monolith targeting .NET 10.

The architecture follows:

- Vertical Slice Architecture
- CQRS
- Event Sourcing
- Event-driven design
- Modular boundaries inside a single deployable backend

Eventuous is the main event sourcing framework.

Slicent is a lightweight framework layer built on top of Eventuous. It does not try to hide Eventuous. Instead, it adds conventions, wiring, and contracts for command handlers, query handlers, vertical slice registration, and related application patterns.

Prefer existing Slicent conventions before introducing new abstractions.

## Infrastructure
Local development is orchestrated with .NET Aspire.

The main infrastructure components are:

- KurrentDB as the event store
- MongoDB as the document/read-model database
- OpenTelemetry for telemetry
- Aspire for local composition and observability

Do not introduce additional infrastructure dependencies without a clear reason and an explicit design note.

## Development Principles

Prefer consistency with the existing codebase over introducing new patterns.

When implementing a feature:

1. Read the relevant specification in `doc/` or feature-local documentation.
2. Inspect similar existing vertical slices.
3. Follow the established command/query/event conventions.
4. Keep the slice cohesive.
5. Add or update tests.
6. Update documentation when behaviour or contracts change.

## Vertical Slices

Features should be implemented as vertical slices.

A vertical slice may include:

- command definitions
- query definitions
- command handlers
- query handlers

Avoid spreading feature logic across generic service layers.

Avoid cross-slice coupling unless there is an explicit shared concept.

## CQRS Rules

Commands mutate state.

Queries read state.

Command handlers should orchestrate behaviour and delegate domain decisions to the appropriate event sourced aggregate

Query handlers should not mutate state.

Do not mix command-side and query-side responsibilities.

## Event Sourcing Rules

Domain changes should be represented as immutable events.

Do not mutate historical events.

Do not introduce CRUD-style persistence for event-sourced concepts.

Domain event names should describe facts that already happened.

Prefer business-meaningful domain events over technical events.

Event schema changes must be backward compatible or include an explicit migration strategy.

## Coding Standards

Use modern .NET 10 and C# conventions.

Prefer:

- explicit names
- immutable data where practical
- small cohesive types
- clear command/query/event names
- async/await end-to-end
- cancellation tokens on async operations where appropriate

Avoid:

- generic repository abstractions unless already established
- anemic domain models
- service-layer orchestration that bypasses slice boundaries
- hidden side effects
- blocking async calls
- unrelated refactors

## Agent Workflow

Before making changes, agents should:

1. Read this file.
2. Read the relevant docs/specs.
3. Inspect the existing implementation style.
4. Make the smallest coherent change.
5. Run or describe the relevant tests.
6. Update docs/specs if behaviour changes.

Agents should not invent missing business rules.

If a business rule is unclear, leave a clear TODO or ask for clarification.