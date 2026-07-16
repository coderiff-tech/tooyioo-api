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

---

# Architecture

This repository is a modular monolith targeting .NET 10.

The architecture follows:

- Vertical Slice Architecture
- CQRS
- Aggregate-less Event Sourcing
- Event-driven design
- Modular boundaries inside a single deployable backend

Eventuous is the primary Event Sourcing framework.

Slicent is a lightweight framework built on top of Eventuous. It intentionally does not abstract Eventuous away. Instead, it provides conventions, wiring, and contracts for command handlers, query handlers, vertical slice registration, and related application patterns.

Prefer existing Slicent conventions before introducing new abstractions.

---

# Core Modeling Principle

This project follows an **aggregate-less Event Sourcing approach**, influenced by Greg Young, Adam Dymitruk and Martin Dilger.

The primary modeling unit is a **business process**, not a business entity.

An event stream represents the execution of a business process rather than the lifetime history of a business object.

The objective is **not** to model everything that has ever happened to an entity. Instead, the objective is to model a single business process with one cohesive purpose and one well-defined consistency boundary.

Each process:

- has a clear beginning and end
- owns one cohesive set of invariants
- defines one consistency boundary
- reconstructs only the state required to make decisions for that process

Streams exist to protect a single consistency boundary.

Long-lived entity streams tend to accumulate unrelated business rules over time, eventually becoming "god aggregates". This modelling style should be avoided.

Although the concept of an aggregate still exists conceptually, explicit aggregate classes are intentionally omitted. Their responsibilities are naturally fulfilled by:

- the event stream
- optimistic concurrency
- process state reconstructed from events
- the command handler

---

# Repository Structure

The main repository folders are:

- `src/` - application and framework source code
- `test/` - automated tests
- `doc/` - documentation, specifications, diagrams, and design notes
- `bruno/` - Bruno collections to interact with the API

Main projects under `src/`:

- `Tooyioo.AppHost` - local development entry point using .NET Aspire
- `Tooyioo.Api` - main ASP.NET Core application, infrastructure, dependency wiring, and `Program.cs`
- `Tooyioo.Common` - shared code that is not generic framework code
- `Slicent` - reusable framework built around Eventuous, Vertical Slice Architecture, CQRS, and Event Sourcing

The application vertical slices are organised under their respective module folders.

---

# Infrastructure

Local development is orchestrated with .NET Aspire.

The main infrastructure components are:

- KurrentDB as the event store
- MongoDB as the document/read-model database
- OpenTelemetry for telemetry
- Aspire for local composition and observability

Do not introduce additional infrastructure dependencies without a clear reason and an accompanying design note.

---

# Development Principles

Prefer consistency with the existing codebase over introducing new patterns.

When implementing a feature:

1. Read the relevant specification under `doc/`.
2. Inspect similar existing vertical slices.
3. Follow the established conventions.
4. Keep the slice cohesive.
5. Add or update tests.
6. Update documentation whenever behaviour or contracts change.

---

# Vertical Slices

Features should be implemented as vertical slices.

A vertical slice may contain:

- command definitions
- query definitions
- command handlers
- query handlers
- state projections
- domain events

Avoid spreading feature logic across generic service layers.

Avoid cross-slice coupling unless there is an explicit shared business concept.

---

# CQRS Rules

Commands mutate state.

Queries read state.

Query handlers must never mutate state.

Command handlers are responsible for evaluating business rules, enforcing invariants, and producing domain events.

The command handler is the **transactional boundary**.

A command handler may:

- reconstruct state from one or more streams
- evaluate business rules
- transactionally append events to one or more streams using the multi-stream append capabilities of Eventuous and KurrentDB

Do not introduce aggregate classes.

The aggregate concept already exists through:

- event streams
- optimistic concurrency
- process state
- consistency boundaries

Do not mix command-side and query-side responsibilities.

---

# Event Sourcing Rules

## Streams Represent Processes

An event stream represents the execution of a business process.

It does **not** represent the lifetime history of a business entity.

Streams must therefore be named using **gerunds**, describing the process being executed.

Examples:

- `UserOnboarding`
- `OrganizationOnboarding`
- `CampaignConfiguring`
- `CampaignRunning`

Avoid entity-centric stream names such as:

- `User`
- `Organization`
- `Campaign`

A stream should answer:

> How is the process currently executing?

not:

> What happened to this object throughout its lifetime?

## State

State reconstructed from a stream is **short-lived process state**.

Its only purpose is to evaluate the business rules required to continue executing that process.

Do not model long-lived entity state when only process state is required.

## Consistency Boundaries

Each process owns exactly one cohesive set of invariants.

Therefore each process defines exactly one consistency boundary.

A stream exists to protect that consistency boundary.

If unrelated invariants begin appearing within the same stream, this is usually a modeling smell indicating multiple business processes have been combined.

Prefer splitting them into separate process streams.

## Domain Events

Domain changes must be represented as immutable events.

Do not mutate historical events.

Do not introduce CRUD persistence for event-sourced concepts.

Domain event names must describe business facts that have already happened.

Prefer business-meaningful events over technical events.

Event schema evolution must remain backward compatible or include an explicit migration strategy.

---

# Coding Standards

Use modern .NET 10 and C# conventions.

Prefer:

- explicit naming
- immutable data where practical
- small cohesive types
- clear command, query, and event names
- async/await end-to-end
- cancellation tokens on asynchronous operations where appropriate

Avoid:

- generic repository abstractions
- anemic domain models
- aggregate classes
- service-layer orchestration that bypasses slice boundaries
- hidden side effects
- blocking asynchronous calls
- unrelated refactoring

---

# Agent Workflow

Before making changes, agents should:

1. Read this file.
2. Read the relevant documentation and specifications.
3. Inspect similar implementations.
4. Make the smallest coherent change.
5. Run, or describe, the relevant tests.
6. Update documentation whenever behaviour changes.

Do not invent missing business rules.

If a business rule is unclear, ask for clarification rather than making assumptions.