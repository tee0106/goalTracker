using Dapper;
using GoalTracker.Api.Models;

namespace GoalTracker.Api.Data;

public sealed class DashboardQueries
{
    private readonly DapperContext _context;

    public DashboardQueries(DapperContext context) => _context = context;

    public async Task<DashboardResponse> GetDashboardAsync()
    {
        await using var connection = _context.CreateConnection();
        var members = (await connection.QueryAsync<MemberRow>(MembersSqlAll)).ToList();
        var goals = (await connection.QueryAsync<GoalRow>(GoalsSqlAll)).ToList();
        var stats = await connection.QuerySingleAsync<StatsRow>(StatsSql);
        return new DashboardResponse(
            members.Select(m => MapMember(m, goals)).ToList(),
            MapStats(stats));
    }

    public async Task<(MemberDto? Member, DashboardStats Stats)> GetMemberSnapshotAsync(int memberId)
    {
        await using var connection = _context.CreateConnection();
        var memberRow = await connection.QuerySingleOrDefaultAsync<MemberRow>(MemberByIdSql, new { memberId });
        if (memberRow is null)
        {
            return (null, new DashboardStats(0, 0, 0, 0));
        }

        var goals = (await connection.QueryAsync<GoalRow>(GoalsByMemberSql, new { memberId })).ToList();
        var stats = await connection.QuerySingleAsync<StatsRow>(StatsSql);
        return (MapMember(memberRow, goals), MapStats(stats));
    }

    private static MemberDto MapMember(MemberRow row, IReadOnlyCollection<GoalRow> goals)
    {
        var memberGoals = goals.Where(g => g.MemberId == row.Id)
            .Select(g => new GoalDto(g.Id, g.Description, g.IsCompleted == 1))
            .ToList();
        var total = (int)(row.TotalCount ?? memberGoals.Count);
        var completed = (int)(row.CompletedCount ?? memberGoals.Count(g => g.IsCompleted));
        return new MemberDto(
            row.Id,
            row.Name,
            row.Emoji ?? string.Empty,
            memberGoals,
            completed,
            total,
            total == 0 ? "No goals yet today" : string.Empty);
    }

    private static DashboardStats MapStats(StatsRow row)
    {
        var completionPercent = row.TotalGoals == 0
            ? 0
            : Math.Round((double)row.CompletedGoals / row.TotalGoals * 100, 1);
        return new DashboardStats(completionPercent, (int)row.HappyCount, (int)row.NeutralCount, (int)row.StressedCount);
    }

    private const string MembersSqlAll =
        """
        WITH today_goals AS (
            SELECT *
            FROM Goals
            WHERE work_date = date('now')
        ),
        today_moods AS (
            SELECT *
            FROM MoodEntries
            WHERE work_date = date('now')
        )
        SELECT tm.id AS Id,
               tm.name AS Name,
               tm.order_index AS OrderIndex,
               COALESCE(m.emoji, '') AS Emoji,
               totals.completed_count AS CompletedCount,
               totals.total_count AS TotalCount
        FROM TeamMembers tm
        LEFT JOIN (
            SELECT member_id,
                   SUM(CASE WHEN is_completed = 1 THEN 1 ELSE 0 END) AS completed_count,
                   COUNT(*) AS total_count
            FROM today_goals
            GROUP BY member_id
        ) totals ON totals.member_id = tm.id
        LEFT JOIN today_moods m ON m.member_id = tm.id
        ORDER BY COALESCE(tm.order_index, 1000), tm.name;
        """;

    private const string MemberByIdSql =
        """
        WITH today_goals AS (
            SELECT *
            FROM Goals
            WHERE work_date = date('now')
        ),
        today_moods AS (
            SELECT *
            FROM MoodEntries
            WHERE work_date = date('now')
        )
        SELECT tm.id AS Id,
               tm.name AS Name,
               tm.order_index AS OrderIndex,
               COALESCE(m.emoji, '') AS Emoji,
               totals.completed_count AS CompletedCount,
               totals.total_count AS TotalCount
        FROM TeamMembers tm
        LEFT JOIN (
            SELECT member_id,
                   SUM(CASE WHEN is_completed = 1 THEN 1 ELSE 0 END) AS completed_count,
                   COUNT(*) AS total_count
            FROM today_goals
            GROUP BY member_id
        ) totals ON totals.member_id = tm.id
        LEFT JOIN today_moods m ON m.member_id = tm.id
        WHERE tm.id = @memberId;
        """;

    private const string GoalsSqlAll =
        """
        SELECT id,
               member_id AS MemberId,
               description,
               is_completed AS IsCompleted
        FROM Goals
        WHERE work_date = date('now')
        ORDER BY member_id, id
        """;

    private const string GoalsByMemberSql =
        """
        SELECT id,
               member_id AS MemberId,
               description,
               is_completed AS IsCompleted
        FROM Goals
        WHERE work_date = date('now') AND member_id = @memberId
        ORDER BY id
        """;

    private const string StatsSql =
        """
        WITH goal_counts AS (
            SELECT SUM(CASE WHEN is_completed = 1 THEN 1 ELSE 0 END) AS completed_goals,
                   COUNT(*) AS total_goals
            FROM Goals
            WHERE work_date = date('now')
        ),
        mood_counts AS (
            SELECT
                SUM(CASE WHEN emoji IN ('😀','😊') THEN 1 ELSE 0 END) AS happy_count,
                SUM(CASE WHEN emoji = '😐' OR emoji IS NULL THEN 1 ELSE 0 END) AS neutral_count,
                SUM(CASE WHEN emoji IN ('😞','😤') THEN 1 ELSE 0 END) AS stressed_count
            FROM TeamMembers tm
            LEFT JOIN MoodEntries me ON me.member_id = tm.id AND me.work_date = date('now')
        )
        SELECT COALESCE(goal_counts.completed_goals, 0) AS CompletedGoals,
               COALESCE(goal_counts.total_goals, 0) AS TotalGoals,
               COALESCE(mood_counts.happy_count, 0) AS HappyCount,
               COALESCE(mood_counts.neutral_count, 0) AS NeutralCount,
               COALESCE(mood_counts.stressed_count, 0) AS StressedCount
        FROM goal_counts, mood_counts
        """;

    private sealed class MemberRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? OrderIndex { get; set; }
        public string? Emoji { get; set; }
        public long? CompletedCount { get; set; }
        public long? TotalCount { get; set; }
    }

    private sealed class GoalRow
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int IsCompleted { get; set; }
    }

    private sealed class StatsRow
    {
        public long CompletedGoals { get; set; }
        public long TotalGoals { get; set; }
        public long HappyCount { get; set; }
        public long NeutralCount { get; set; }
        public long StressedCount { get; set; }
    }
}


