# Data Model: Core Dashboard MVP

Date: 2025-11-19  
Spec: `specs/001-core-dashboard/spec.md`

---

## Entity: TeamMember
- **Primary Key**: `id` (INTEGER, autoincrement)
- **Fields**:
  - `name` (TEXT, required, unique per team)
  - `role` (TEXT, optional, for context only)
  - `order_index` (INTEGER, optional, controls card ordering)
  - `created_at` (DATETIME, default now)
- **Relationships**:
  - `TeamMember 1 ── * Goal`
  - `TeamMember 1 ── 1 MoodEntry (current day)`
- **Validation Rules**:
  - Names trimmed, max 60 chars.
  - Order index non-negative if present.

## Entity: Goal
- **Primary Key**: `id` (INTEGER)
- **Fields**:
  - `member_id` (FK → TeamMember.id, required, cascade delete)
  - `description` (TEXT, required, 1–250 chars)
  - `is_completed` (BOOLEAN, default false)
  - `work_date` (DATE, default `current_date`)
  - `created_at` / `updated_at` (DATETIME)
- **State Transitions**:
  - `pending` → `completed` via checkbox toggle.
  - `completed` → `pending` if checkbox unchecked (allowed for reopened work).
- **Indexes**:
  - `(member_id, work_date)` covering index for fast dashboard lookups.
  - Partial index on `work_date = current_date` for daily truncation queries.

## Entity: MoodEntry
- **Primary Key**: `id` (INTEGER)
- **Fields**:
  - `member_id` (FK → TeamMember.id, unique with `work_date`)
  - `emoji` (TEXT, allowed values 😀 😊 😐 😞 😤)
  - `work_date` (DATE, default `current_date`)
  - `recorded_at` (DATETIME)
- **Constraints**:
  - Unique index `(member_id, work_date)` ensures only latest row per day; updating mood overwrites the row.
- **Notes**:
  - Dashboard derives mood buckets: 😀/😊 = Happy, 😐 = Neutral, 😞/😤 = Stressed.

## Derived View: DashboardSnapshot (not stored)
- **Composition**: Result of joining TeamMember with current-day Goals and MoodEntry.
- **Shape**:
  - `members[]`: each contains `id`, `name`, `emoji`, `goals[]` (id, description, is_completed), `completedCount`, `totalCount`.
  - `stats`: `completionPercent`, `happyCount`, `neutralCount`, `stressedCount`.
- **Computation**:
  - Completion percent = completed goals ÷ total goals across all members; guard against divide-by-zero.
  - Missing mood defaults to “No mood logged” and counts as Neutral until updated.

## Processes
- **Daily Reset**: Truncate Goals and MoodEntry rows where `work_date < current_date`.
- **Optimistic Update Flow**:
  - Client sends mutation; backend updates row then re-queries DashboardSnapshot to return updated payload.

