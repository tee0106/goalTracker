using Microsoft.Data.Sqlite;

namespace GoalTracker.Api.Data;

public sealed class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(DatabaseConfig config) => _connectionString = config.ConnectionString;

    public SqliteConnection CreateConnection() => new(_connectionString);
}


