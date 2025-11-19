# Implementation Plan: Core Dashboard MVP

**Branch**: `001-core-dashboard` | **Date**: 2025-11-19 | **Spec**: `specs/001-core-dashboard/spec.md`
**Input**: Problem Statement & Product Overview from `requirements.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

Teams currently lack a single place to see daily goals and morale; coaches cannot tell who is blocked or needs encouragement (Problem Statement). The GoalTracker MVP delivers one desktop dashboard where every member card displays goals, completion counts, and mood emoji while lightweight forms allow adding goals and updating moods (Product Overview). We will keep the stack minimal—Vue 3 + DaisyUI frontend, .NET 8 Web API + Dapper backend, SQLite storage—to satisfy the constitution and “fewer lines of code” mandate. Research confirmed that a single `/api/dashboard` snapshot, optimistic UI, and daily data truncation provide the required visibility without overbuilding.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: Frontend → Vue 3 + TypeScript; Backend → .NET 8 Web API  
**Primary Dependencies**: Vite, Tailwind + DaisyUI, Dapper, SQLite driver  
**Storage**: SQLite file (SQL Server in Docker only if explicitly required)  
**Testing**: None (manual verification only)  
**Target Platform**: Desktop browser + local dev servers
**Project Type**: Web app (frontend + backend)  
**Performance Goals**: Instant dashboard loads for <100 goals; <200ms API responses  
**Constraints**: Desktop-only UX, single-day scope, no background schedulers  
**Scale/Scope**: Single small team; keep codepaths minimal and focused

## Constitution Check (Pre-Design)

- ✅ Scope: All planned work (dashboard view, Add Goal, Update Mood, stats) maps directly to MVP feature list.
- ✅ Frontend stack: Vue 3 + Composition API + DaisyUI only; no alternate UI libraries.
- ✅ Backend stack: .NET 8 Web API controllers with Dapper queries; no ORMs.
- ✅ Persistence: SQLite file with optional manual truncate; SQL Server not needed.
- ✅ Quality: Manual verification checklist replaces tests (`quickstart.md`).

*Gate result*: PASS — proceed to Phase 0 research.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
backend/
├── src/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   └── Services/
└── GoalTracker.Api.csproj

frontend/
├── src/
│   ├── components/
│   ├── composables/
│   ├── pages/
│   └── stores/
└── vite.config.ts
```

**Structure Decision**: Reference the concrete backend/frontend directories above; keep all work
within those roots.

## Complexity Tracking

No deviations from the constitution or template structure are required at this time.

---

## Phase 0 – Research Summary

All unknowns from Technical Context were resolved; see `research.md` for details.

| Topic | Decision | Impact |
|-------|----------|--------|
| Dashboard data shape | Backend `/api/dashboard` aggregates members, moods, goals, stats | Frontend fetches once per poll and stays thin |
| Freshness strategy | Optimistic UI + 15 s polling | Eliminates need for sockets while keeping morale data timely |
| Data retention | Store only today’s rows with nightly truncate | Keeps schema simple and matches “single-day scope” |

*Outstanding clarifications*: None. Proceed to design.

---

## Phase 1 – Solution Design

### Frontend (Vue 3 + DaisyUI)
- Structure: `frontend/src/pages/DashboardPage.vue` orchestrates cards, forms, and stats.
- Shared state: `useDashboardStore` composable encapsulates fetch, optimistic mutations, and 15 s polling timer.
- Components:
  - `MemberCard.vue`: name, emoji, helper text “No goals yet today”, checklist items with inline toggle.
  - `AddGoalForm.vue`, `UpdateMoodForm.vue`: simple DaisyUI card forms with dropdown + inputs.
  - `StatsPanel.vue`: displays completion percent (completed ÷ total) and mood counts using clarified emoji buckets.
- Error UX: Toast/banner when mutations fail; revert optimistic change and prompt retry.

### Backend (.NET 8 + Dapper)
- Controllers:
  - `DashboardController.GetDashboard()` → returns `DashboardResponse` DTO.
  - `GoalsController.PostGoal()` / `PatchGoal()` for creation + completion toggle.
  - `MoodController.PostMood()` for emoji updates.
- Data layer:
  - `DashboardQueries.cs` uses CTEs to join TeamMembers, Goals, MoodEntries filtered by `work_date = current_date`.
  - Repository functions kept minimal; raw SQL stored in `/backend/src/Data/sql/`.
- Validation:
  - Server enforces description length, allowed emoji, member existence, same-day guard.
  - Toggle endpoint ensures goal belongs to today before updating.

### Persistence (SQLite)
- Tables: `TeamMembers`, `Goals`, `MoodEntries` as defined in `data-model.md`.
- Seed script provides 4–6 members with starting goals to support demos.
- Maintenance command to truncate rows older than today (manual CLI).

### Manual Verification (No Automated Tests)
- `quickstart.md` enumerates desktop-only smoke steps covering the three user stories plus failure-path validation.
- Each feature branch PR must demonstrate manual walkthrough evidence (screenshots/logs).

### Risks & Mitigations
- **Stale data**: 15 s polling plus manual refresh button.
- **Concurrent edits**: Backend uses latest write-wins; optimistic revert on 409.
- **Single DB file corruption**: Document backup instructions (copy file daily) as part of ops notes.

---

## Phase 2 – Implementation Outline

1. **Backend foundation**
   - Scaffold controllers + DTOs.
   - Add Dapper queries and migrations for the three tables.
   - Implement `/api/dashboard`, `/api/goals`, `/api/moods`.
2. **Frontend dashboard**
   - Build composable store with fetch + polling.
   - Render member cards + stats, wire checkbox toggle.
3. **Forms + validation**
   - Implement Add Goal and Update Mood forms with DaisyUI components.
   - Hook to mutations with optimistic updates + error states.
4. **Manual verification assets**
   - Update `quickstart.md` as flows stabilize.
   - Capture screenshots/log outputs for review.

*Next command after plan*: `/speckit.tasks` to derive executable work items from this plan.
## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
|           |            |                                     |
