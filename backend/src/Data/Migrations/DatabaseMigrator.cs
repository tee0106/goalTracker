using Dapper;
using Microsoft.Data.Sqlite;

namespace GoalTracker.Api.Data.Migrations;

internal sealed class DatabaseMigrator
{
    private readonly DatabaseConfig _config;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(DatabaseConfig config, ILogger<DatabaseMigrator> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(_config.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(InitialMigration.Script);
        _logger.LogInformation("✅ Database ready at {Path}", _config.FilePath);
    }
}

internal static class MigrationServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseMigrations(this IServiceCollection services)
    {
        services.AddSingleton<DatabaseMigrator>();
        return services;
    }

    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigrator>();
        await migrator.EnsureCreatedAsync(CancellationToken.None);
    }
}

