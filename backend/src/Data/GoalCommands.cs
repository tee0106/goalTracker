using Dapper;
using GoalTracker.Api.Models;

namespace GoalTracker.Api.Data;

public sealed class GoalCommands
{
    private readonly DapperContext _context;
    private readonly DashboardQueries _dashboard;

    public GoalCommands(DapperContext context, DashboardQueries dashboard)
    {
        _context = context;
        _dashboard = dashboard;
    }

    public async Task<GoalResponse?> AddGoalAsync(AddGoalRequest request)
    {
        await using var connection = _context.CreateConnection();
        const string insertSql =
            """
            INSERT INTO Goals (member_id, description, is_completed, work_date, created_at, updated_at)
            VALUES (@MemberId, @Description, 0, date('now'), datetime('now'), datetime('now'));
            """;
        var affected = await connection.ExecuteAsync(insertSql, request);
        if (affected == 0)
        {
            return null;
        }

        var snapshot = await _dashboard.GetMemberSnapshotAsync(request.MemberId);
        return snapshot.Member is null
            ? null
            : new GoalResponse(snapshot.Member, snapshot.Stats);
    }

    public async Task<GoalResponse?> ToggleGoalAsync(int goalId, bool isCompleted)
    {
        await using var connection = _context.CreateConnection();
        const string updateSql =
            """
            UPDATE Goals
            SET is_completed = @isCompleted,
                updated_at = datetime('now')
            WHERE id = @goalId AND work_date = date('now');
            """;

        var updated = await connection.ExecuteAsync(updateSql, new { goalId, isCompleted = isCompleted ? 1 : 0 });
        if (updated == 0)
        {
            return null;
        }

        var memberId = await connection.ExecuteScalarAsync<int?>(
            "SELECT member_id FROM Goals WHERE id = @goalId",
            new { goalId });
        if (memberId is null)
        {
            return null;
        }

        var snapshot = await _dashboard.GetMemberSnapshotAsync(memberId.Value);
        return snapshot.Member is null
            ? null
            : new GoalResponse(snapshot.Member, snapshot.Stats);
    }
}


