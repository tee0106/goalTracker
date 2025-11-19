# GoalTracker – Core Dashboard MVP

## Setup
1. Copy `.env.example` → `.env` and confirm:
   - `API_PORT=5000`
   - `FRONTEND_PORT=5173`
   - `DB_FILE=backend/data/goaltracker.db`
2. Install dependencies:
   - Backend: `cd backend && dotnet restore`
   - Frontend: `cd frontend && npm install`

## Run
| Service  | Command |
|----------|---------|
| API      | `cd backend && dotnet run` |
| Frontend | `cd frontend && npm run dev` |

The Vue app reads `VITE_API_BASE_URL` (default `http://localhost:5000`) for API calls.

## Seed + Reset Helpers
- Seed demo data once: `curl -XPOST http://localhost:5000/api/debug/seed`
- Truncate previous-day rows: `backend/scripts/reset-today.ps1` (wraps `dotnet run -- reset-today`)

## Manual Verification
- Follow `specs/001-core-dashboard/quickstart.md` for full walkthrough, including evidence tracker links under `docs/evidence/`.
- Capture dashboard screenshot + console logs before shipping each branch.

