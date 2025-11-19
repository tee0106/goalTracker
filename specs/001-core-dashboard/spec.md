# Feature Specification: Core Dashboard MVP

**Feature Branch**: `001-core-dashboard`  
**Created**: 2025-11-19  
**Status**: Draft  
**Input**: User description: "Please read CORE Features (MVP Only) from @requirements.md"

## User Scenarios & Manual Verification *(mandatory)*

<!--
  IMPORTANT: User stories should be PRIORITIZED as user journeys ordered by importance.
  Each user story/journey must deliver value on its own so the dashboard stays shippable after
  every increment.
  
  Assign priorities (P1, P2, P3, etc.) to each story, where P1 is the most critical.
  Think of each story as a standalone slice of functionality that can be:
  - Developed independently
  - Manually checked independently
  - Deployed independently
  - Demonstrated to users independently
-->

### User Story 1 - View Daily Goals & Mood (Priority: P1)

Managers and teammates open the dashboard each morning to see every member’s current mood, open goals, and completion status in one place.

**Why this priority**: Without an at-a-glance board, leaders cannot spot blockers or celebrate wins early in the day.

**Manual Verification**: Load the dashboard with seeded members; confirm each card shows name, mood emoji, goal list, and completion counter. Toggle a goal checkbox and ensure the card count and stats panel update immediately.

**Acceptance Scenarios**:

1. **Given** team data for the current day, **When** the dashboard loads, **Then** each member card displays name, latest mood emoji, list of goals with checkboxes, and a “completed/total” counter.
2. **Given** a member card with incomplete goals, **When** a user marks a goal complete via the checkbox, **Then** the completion counter increments and the stats panel refreshes without leaving the page.

---

### User Story 2 - Add Daily Goals (Priority: P2)

Coordinators need to capture new goals for any teammate throughout the day using a simple form.

**Why this priority**: Team goals shift frequently; adding them quickly ensures the dashboard reflects real work.

**Manual Verification**: Open the Add Goal form, choose a member from the dropdown, enter a description, submit, and confirm the new goal appears under the selected member card with the total count updated.

**Acceptance Scenarios**:

1. **Given** the Add Goal form with member dropdown and text field, **When** a user selects a member and submits a non-empty description, **Then** the goal is saved and instantly displayed under that member’s card with a cleared form ready for the next entry.
2. **Given** the form, **When** the user attempts to submit without selecting a member or entering text, **Then** the system blocks submission and shows a helpful validation message.

---

### User Story 3 - Update Mood (Priority: P3)

Any teammate can report their current mood emoji via the dashboard so the team can spot morale swings.

**Why this priority**: Real-time mood visibility enables quick support when someone is neutral or stressed.

**Manual Verification**: Use the Update Mood dropdown to select a member and a mood emoji; submit and check the member card and stats panel reflect the new emoji and mood counts immediately.

**Acceptance Scenarios**:

1. **Given** the Update Mood form with member dropdown and emoji selector, **When** a user submits a mood, **Then** the member’s card updates to the selected emoji and the mood counts adjust accordingly.
2. **Given** a member who already has a mood logged, **When** a new mood is submitted, **Then** the prior mood is replaced and the stats panel rebalances the counts without duplicates.

---

[Add more user stories as needed, each with an assigned priority]

### Edge Cases

- No goals yet for a member: their card still appears with “0/0 goals” and an empty list.
- All goals completed: completion counter shows full value and checkboxes render checked but still allow unchecking if a task reopens.
- Duplicate mood submissions: the latest submission overrides the prior entry without stacking multiple moods.
- Invalid goal description (blank or >250 chars): form displays an inline error and prevents save.
- Add goal/update mood while data refresh in progress: optimistic UI shows spinner until confirmation to avoid conflicting states.
- Dashboard load failure: show a friendly error and retry CTA instead of a blank page.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The dashboard MUST list every active team member in card format that includes name, latest mood emoji, current goals, and a “completed/total” counter.
- **FR-002**: Each card MUST allow users to mark individual goals complete or incomplete via checkboxes and immediately update the counter.
- **FR-003**: The Add Goal form MUST provide a team-member dropdown plus a text field, enforce non-empty descriptions, and append new goals to the selected card upon submission.
- **FR-004**: The Update Mood form MUST offer the same team-member dropdown plus the five emoji options (😀 😊 😐 😞 😤) and persist the latest selection per member.
- **FR-005**: All goal additions, completions, and mood updates MUST reflect in the stats panel and related cards without requiring a full page reload.
- **FR-006**: The stats panel MUST show overall team goal completion percentage for the current day.
- **FR-007**: The stats panel MUST list counts of members who are happy, neutral, or stressed based on their most recent mood emoji.
- **FR-008**: The system MUST validate and store all updates against the current day so historical data does not accumulate in the MVP.
- **FR-009**: If a save operation fails, the UI MUST surface a clear error message and leave the prior values unchanged.

### Key Entities

- **Team Member**: Represents an individual on the team; attributes include name, role (optional), and current mood emoji; links to goals.
- **Goal**: Daily objective tied to exactly one team member; attributes include text description, completion flag, and day stamp.
- **Mood Entry**: Latest mood status captured per member per day; attributes include emoji value, timestamp, and optional note.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Dashboard load shows every team member’s card with accurate data within 2 seconds on a standard laptop for teams up to 12 people.
- **SC-002**: 100% of manual goal completion attempts update the member card and stats panel without page reloads.
- **SC-003**: 95% of manual Add Goal submissions succeed on the first try when required fields are filled, and validation errors display in under 1 second when fields are missing.
- **SC-004**: Mood submissions reflect in both the member card and mood counts within 1 second for 95% of manual walkthroughs.
- **SC-005**: Team leads report (via internal survey) that they can determine who needs help or kudos within 30 seconds of viewing the dashboard.

## Assumptions & Dependencies

- Team roster exists ahead of time; MVP does not include creating or deleting members.
- Workday resets occur manually each morning; clearing prior goals/moods is handled outside this feature.
- Users share a single desktop dashboard (no authentication or personalization needed).
