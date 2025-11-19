using Dapper;
using GoalTracker.Api.Models;

namespace GoalTracker.Api.Data;

public sealed class MoodCommands
{
    private readonly DapperContext _context;
    private readonly DashboardQueries _dashboard;

    public MoodCommands(DapperContext context, DashboardQueries dashboard)
    {
        _context = context;
        _dashboard = dashboard;
    }

    public async Task<MoodResponse?> UpdateMoodAsync(UpdateMoodRequest request)
    {
        await using var connection = _context.CreateConnection();
        const string upsertSql =
            """
            INSERT INTO MoodEntries (member_id, emoji, work_date, recorded_at)
            VALUES (@MemberId, @Emoji, date('now'), datetime('now'))
            ON CONFLICT(member_id, work_date)
            DO UPDATE SET emoji = excluded.emoji,
                          recorded_at = excluded.recorded_at;
            """;
        var affected = await connection.ExecuteAsync(upsertSql, request);
        if (affected == 0)
        {
            return null;
        }

        var stats = await _dashboard.GetMemberSnapshotAsync(request.MemberId);
        return stats.Member is null
            ? null
            : new MoodResponse(request.MemberId, request.Emoji, stats.Stats);
    }
}


