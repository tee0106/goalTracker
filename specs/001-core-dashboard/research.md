# Research: Core Dashboard MVP

Date: 2025-11-19  
Spec: `specs/001-core-dashboard/spec.md`

---

## Decision 1: Backend computes dashboard snapshot per request
- **Decision**: Expose a `/api/dashboard` endpoint that returns all members, goals, and stats for the current day in one payload.
- **Rationale**: Managers need a single view of mood + goals (Problem Statement). Generating stats server-side keeps the frontend light and prevents duplicated percentage/mood math.
- **Alternatives considered**:
  - Client-side aggregation after fetching multiple lists → more network roundtrips and increases Vue complexity.
  - Push-based streaming (SignalR/WebSockets) → unnecessary for desktop MVP and contradicts “fewer lines of code”.

## Decision 2: Optimistic UI + 15s polling for freshness
- **Decision**: Apply optimistic updates on goal/mood mutations and re-fetch the dashboard snapshot every 15 seconds (configurable) to reconcile state.
- **Rationale**: MVP only needs near-real-time awareness; polling is trivial to implement and aligns with “minimal full-stack” guidance. Optimistic updates keep UX responsive even without sockets.
- **Alternatives considered**:
  - Manual page refresh only → risks stale morale data if users forget to reload.
  - Persistent WebSocket → higher infra complexity and more code to maintain sessions.

## Decision 3: Store only “today” rows with nightly truncate
- **Decision**: Goals and moods tables keep a `work_date` column defaulting to today; a simple maintenance job (manual script) truncates rows older than the current day.
- **Rationale**: Requirements explicitly limit scope to single-day tracking and forbid history. Including the date still allows midday resets without schema churn.
- **Alternatives considered**:
  - Omitting the date column entirely → makes it harder to reset when the day rolls over.
  - Building full history/migrations → out of scope and increases storage footprint for no immediate value.

