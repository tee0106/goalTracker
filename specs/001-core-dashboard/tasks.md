# Tasks: Core Dashboard MVP

**Input**: Design documents from `/specs/001-core-dashboard/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/, quickstart.md

**Verification**: Automated tests are disallowed. Capture manual verification steps for every story.

**Organization**: Tasks are grouped by user story to enable independent implementation and manual validation of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Web app**: `backend/src/`, `frontend/src/`
- Paths shown below follow plan.md structure

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Repository prep and shared tooling

- [X] T001 Initialize frontend dependencies and DaisyUI config in `frontend/`
- [X] T002 Initialize backend dependencies and SQLite connection in `backend/`
- [X] T003 [P] Configure shared `.env.example` + README instructions for API/DB ports

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Backend data + API scaffolding that all stories rely on  
**⚠️ CRITICAL**: Complete before any user story work

- [X] T004 Create SQLite schema & migrations for TeamMembers/Goals/MoodEntries in `backend/src/Data/Migrations/`
- [X] T005 [P] Seed script + `/api/debug/seed` endpoint in `backend/src/Controllers/DebugController.cs`
- [X] T006 Scaffold `DashboardResponse`, `GoalDto`, `MoodDto` models in `backend/src/Models/`
- [X] T007 Implement Dapper context + helper methods in `backend/src/Data/DapperContext.cs`
- [X] T008 Add base fetch composable `frontend/src/composables/useApi.ts` with fetch + error helpers
- [X] T009 [P] Create manual verification checklist entry in `specs/001-core-dashboard/quickstart.md` (placeholder for later steps)

---

## Phase 3: User Story 1 – View Daily Goals & Mood (Priority: P1) 🎯 MVP

**Goal**: Render dashboard of member cards with real-time stats.  
**Manual Verification**: Load dashboard, confirm member cards show name, emoji, helper text, goal list, and stats update when toggling checkboxes.

- [X] T010 [US1] Backend query: implement `/api/dashboard` joining members/goals/moods in `backend/src/Controllers/DashboardController.cs`
- [X] T011 [US1] Add SQL + repository logic for dashboard snapshot in `backend/src/Data/DashboardQueries.cs`
- [X] T012 [P] [US1] Create `useDashboardStore.ts` with fetch + 15s polling + optimistic caching in `frontend/src/stores/`
- [X] T013 [P] [US1] Build `MemberCard.vue` displaying helper text when `totalCount === 0` in `frontend/src/components/`
- [X] T014 [US1] Build `StatsPanel.vue` computing completion percent (completed ÷ total) and mood buckets in `frontend/src/components/`
- [X] T015 [US1] Assemble `DashboardPage.vue` to render cards, stats, and wire checkbox toggles in `frontend/src/pages/`
- [X] T016 [US1] Document manual walkthrough + screenshots for US1 in `specs/001-core-dashboard/quickstart.md`

**Checkpoint**: US1 working independently with manual evidence.

---

## Phase 4: User Story 2 – Add Daily Goals (Priority: P2)

**Goal**: Coordinators can append goals to any member mid-day.  
**Manual Verification**: Submit Add Goal form; goal appears immediately under selected member with updated counter + helper text removed.

- [X] T017 [US2] Backend: implement POST `/api/goals` endpoint + validation in `backend/src/Controllers/GoalsController.cs`
- [X] T018 [P] [US2] Add Dapper command to insert goal + return refreshed member snapshot in `backend/src/Data/GoalCommands.cs`
- [X] T019 [US2] Create `AddGoalForm.vue` with member dropdown + validation in `frontend/src/components/forms/`
- [X] T020 [US2] Wire form submission + optimistic update in `useDashboardStore.ts`
- [X] T021 [US2] Extend quickstart manual checklist with Add Goal flow evidence in `specs/001-core-dashboard/quickstart.md`

**Checkpoint**: US2 independently verifiable and does not block US1.

---

## Phase 5: User Story 3 – Update Mood (Priority: P3)

**Goal**: Any teammate can update their mood emoji throughout the day.  
**Manual Verification**: Submit mood update form; card emoji and mood counts change instantly, multiple submissions allowed.

- [X] T022 [US3] Backend: implement POST `/api/moods` replacing same-day mood row in `backend/src/Controllers/MoodController.cs`
- [X] T023 [P] [US3] Add Dapper upsert for mood entry + stats recalculation in `backend/src/Data/MoodCommands.cs`
- [X] T024 [US3] Create `UpdateMoodForm.vue` with emoji selector + validation in `frontend/src/components/forms/`
- [X] T025 [US3] Connect mood submission + optimistic state update in `useDashboardStore.ts`
- [X] T026 [US3] Update quickstart manual checklist with mood flow evidence in `specs/001-core-dashboard/quickstart.md`

**Checkpoint**: US3 functional and independently verified.

---

## Phase N: Polish & Cross-Cutting Concerns

- [X] T027 [P] Refresh documentation (`README.md` + `quickstart.md`) with setup + verification notes
- [X] T028 Add retry + offline banner when dashboard fetch fails in `frontend/src/components/`
- [X] T029 [P] Add CLI helper to truncate previous-day data in `backend/scripts/reset-today.ps1`
- [X] T030 Performance smoke: profile `/api/dashboard` query and document findings in `specs/001-core-dashboard/research.md`
- [X] T031 [P] Capture final manual walkthrough evidence (screens/video) in `docs/evidence/`

---

## Dependencies & Execution Order

- **Phase Dependencies**
  - Setup → Foundational → User Stories (US1 → US2 → US3) → Polish
  - US2 and US3 depend on Foundational + US1 data structures but can proceed after their prerequisites independently.

- **Parallel Opportunities**
  - Setup: T001/T002 sequential, T003 parallel
  - Foundational: T004–T008 mostly sequential; T005/T008 parallel
  - US1: Frontend components (T012–T015) can proceed in parallel once T010/T011 contract exists
  - US2/US3: Frontend form build (T019/T024) parallel with backend mutations (T017/T022)

- **Manual Verification**
  - Each US phase includes a task to update `quickstart.md` with walkthrough + evidence (T016, T021, T026).

## Implementation Strategy

1. Complete Setup + Foundational tasks.
2. Deliver US1 (dashboard view) → manual verification = MVP.
3. Layer US2 (Add Goal) → manual verification.
4. Layer US3 (Update Mood) → manual verification.
5. Finish with Polish tasks for documentation, resilience, and tooling.

