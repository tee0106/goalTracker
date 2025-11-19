using Dapper;

namespace GoalTracker.Api.Data;

public sealed class SeedData
{
    private readonly DapperContext _context;
    private readonly ILogger<SeedData> _logger;

    private static readonly string[] DefaultMembers =
    [
        "Avery (Coach)",
        "Jordan",
        "Kai",
        "Logan",
        "Riley"
    ];

    private static readonly string[] StarterGoals =
    [
        "Pair on blockers",
        "Ship dashboard polish",
        "Prep demo talking points"
    ];

    public SeedData(DapperContext context, ILogger<SeedData> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SeedAsync(CancellationToken cancellationToken)
    {
        await using var connection = _context.CreateConnection();
        var existing = await connection.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TeamMembers");
        if (existing > 0)
        {
            _logger.LogInformation("Seed skipped: members already exist.");
            return 0;
        }

        await connection.OpenAsync(cancellationToken);
        await using var tx = await connection.BeginTransactionAsync(cancellationToken);

        const string memberSql =
            "INSERT INTO TeamMembers (name, role, order_index) VALUES (@name, NULL, @order)";
        for (var i = 0; i < DefaultMembers.Length; i++)
        {
            await connection.ExecuteAsync(memberSql, new { name = DefaultMembers[i], order = i }, tx);
        }

        var memberIds = await connection.QueryAsync<int>("SELECT id FROM TeamMembers ORDER BY order_index", transaction: tx);
        const string goalSql =
            """
            INSERT INTO Goals (member_id, description, is_completed, work_date, created_at, updated_at)
            VALUES (@memberId, @description, @isCompleted, date('now'), datetime('now'), datetime('now'));
            """;
        foreach (var memberId in memberIds)
        {
            foreach (var description in StarterGoals)
            {
                await connection.ExecuteAsync(goalSql, new { memberId, description, isCompleted = 0 }, tx);
            }
        }

        await tx.CommitAsync(cancellationToken);
        _logger.LogInformation("Seed inserted {Count} members", DefaultMembers.Length);
        return DefaultMembers.Length;
    }
}


