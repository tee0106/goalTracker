<!--
Sync Impact Report
- Version: none → 1.0.0
- Modified principles: (template placeholders) → Dashboard Simplicity, Vue 3 + DaisyUI, .NET 8 + Dapper, SQLite Local Data, Manual Verification Only
- Added sections: Scope Guardrails, Delivery Workflow, Governance policy
- Removed sections: Template placeholders
- Templates requiring updates: .specify/templates/plan-template.md ✅, .specify/templates/spec-template.md ✅, .specify/templates/tasks-template.md ✅
- Follow-up TODOs: none
-->
# GoalTracker Constitution

## Core Principles

### P1. Dashboard-First Simplicity
Deliver a single desktop dashboard that lists team members, their moods, and their daily goals.
Every feature MUST directly support logging moods, adding goals, checking completion counts,
or showing team-wide stats—extra pages, history views, and visualizations stay out.

### P2. Vue 3 + DaisyUI Frontend
All UI work MUST use Vue 3 with the Composition API, TypeScript strict mode, Tailwind, and DaisyUI
components. Keep client logic thin, reuse composables for shared state, and avoid alternative UI
libraries or CSS frameworks that duplicate DaisyUI capabilities.

### P3. .NET 8 Web API with Dapper
Backend code MUST run on .NET 8 Web API endpoints with Dapper for every data access. Avoid ORMs or
middleware layers that obscure SQL; favor concise parameterized queries and DTOs tailored to the
dashboard payloads to keep latency low.

### P4. SQLite Local Persistence
Use SQLite as the default datastore (file-backed, local). If SQL Server is ever required, it MUST run
in Docker with identical schema and Dapper mappings. Keep schema minimal—tables for team members,
goals, and moods only—and design seed data that keeps local bootstrapping trivial.

### P5. Manual Verification Only
Automated tests (unit, integration, e2e, contract) are disallowed. Authors MUST rely on manual
walkthroughs of the dashboard flows after each change. Any tooling, CI jobs, or templates that would
generate automated tests MUST stay disabled or be removed.

## Scope Guardrails

The product excludes authentication, multi-day history, analytics, notifications, admin tooling,
goal editing beyond add/complete/delete, recurring goals, categories, responsive/mobile layouts,
dark mode, profile pages, or any feature not explicitly listed in the MVP requirements. Enforce
these omissions during planning and reviews to keep velocity high and UX focused.

## Delivery Workflow

Keep a single web frontend (`frontend/`) and backend (`backend/`) codepath. Frontend tasks implement
Vue 3 components/composables, while backend tasks expose the requisite REST endpoints plus Dapper
queries. Each feature branch MUST document manual verification steps instead of tests. Deployment is
local-only: `npm run dev` for frontend and `dotnet run` for backend.

## Governance

- This constitution supersedes ad-hoc practices; reviewers MUST confirm compliance before merging.
- Amendments require consensus on scope/stack changes plus simultaneous template updates.
- Versioning follows semantic rules: major for breaking governance, minor for new principles or
  sections, patch for clarifications.
- Ratified guidance remains in force until superseded; compliance reviews occur per feature.

**Version**: 1.0.0 | **Ratified**: 2025-11-19 | **Last Amended**: 2025-11-19
