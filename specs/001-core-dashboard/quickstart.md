# Quickstart: Core Dashboard MVP

Date: 2025-11-19  
Branch: `001-core-dashboard`

---

## Prerequisites
- Node 20+, pnpm or npm
- .NET 8 SDK
- SQLite CLI (optional for inspecting DB file)

## 1. Install & Run
1. `cd frontend && npm install`
2. `cd backend && dotnet restore`
3. Start backend API: `cd backend && dotnet run`
4. Start frontend: `cd frontend && npm run dev`
5. Visit `http://localhost:5173`

## 2. Seed Data (optional)
- Backend exposes `/api/debug/seed`. Call once via `curl -XPOST http://localhost:5000/api/debug/seed` to insert sample team members and goals for the current day.

## 3. Manual Verification Checklist
1. **Dashboard load**: Refresh the page; confirm all members render with name, mood emoji, helper text “No goals yet today” for empty lists, and the stats panel shows overall completion percentage and mood counts.
2. **Add goal**:
   - Use the Add Goal form, select a member, provide a description, submit.
   - Expected: Form clears, helper text disappears if first goal, counter increments, stats update.
3. **Toggle goal completion**:
   - Check/uncheck a goal.
   - Expected: `completed/total` counter updates instantly, stats panel percentage adjusts.
4. **Update mood**:
   - Choose a member + emoji in Update Mood form.
   - Expected: Card emoji swaps to new icon, mood bucket counts shift based on mapping (😀/😊 Happy, 😐 Neutral, 😞/😤 Stressed).
5. **Polling refresh**:
   - Wait 15 seconds without touching UI; confirm dashboard silently refreshes (network call visible in devtools) and retains optimistic changes.
6. **Failure handling**:
   - Kill backend process and attempt Add Goal; expect inline error “Unable to save right now” and prior data unchanged. Restart backend afterwards.

## 4. Resetting Day
- Run `dotnet run --project backend/GoalTracker.Api.csproj -- reset-today` (CLI helper) or execute SQL `DELETE FROM Goals; DELETE FROM MoodEntries;` to clear current-day data before next demo.

