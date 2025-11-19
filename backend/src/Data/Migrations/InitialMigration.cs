namespace GoalTracker.Api.Data.Migrations;

internal static class InitialMigration
{
    public const string Script =
        """
        PRAGMA foreign_keys = ON;

        CREATE TABLE IF NOT EXISTS TeamMembers
        (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL UNIQUE,
            role TEXT,
            order_index INTEGER,
            created_at TEXT NOT NULL DEFAULT (datetime('now'))
        );

        CREATE TABLE IF NOT EXISTS Goals
        (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            member_id INTEGER NOT NULL,
            description TEXT NOT NULL CHECK (length(description) BETWEEN 1 AND 250),
            is_completed INTEGER NOT NULL DEFAULT 0,
            work_date TEXT NOT NULL DEFAULT (date('now')),
            created_at TEXT NOT NULL DEFAULT (datetime('now')),
            updated_at TEXT NOT NULL DEFAULT (datetime('now')),
            FOREIGN KEY (member_id) REFERENCES TeamMembers(id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS MoodEntries
        (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            member_id INTEGER NOT NULL,
            emoji TEXT NOT NULL CHECK (emoji IN ('😀','😊','😐','😞','😤')),
            work_date TEXT NOT NULL DEFAULT (date('now')),
            recorded_at TEXT NOT NULL DEFAULT (datetime('now')),
            FOREIGN KEY (member_id) REFERENCES TeamMembers(id) ON DELETE CASCADE,
            UNIQUE(member_id, work_date)
        );

        CREATE INDEX IF NOT EXISTS idx_goals_member_date ON Goals(member_id, work_date);
        CREATE INDEX IF NOT EXISTS idx_goals_work_date ON Goals(work_date);
        CREATE INDEX IF NOT EXISTS idx_goals_today_completed ON Goals(work_date, is_completed);
        CREATE INDEX IF NOT EXISTS idx_mood_work_date ON MoodEntries(work_date);
        """;
}

