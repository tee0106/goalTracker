using Dapper;
using GoalTracker.Api.Data;
using GoalTracker.Api.Data.Migrations;
using Microsoft.Data.Sqlite;

var resetMode = args.Any(a => string.Equals(a, "reset-today", StringComparison.OrdinalIgnoreCase));
var filteredArgs = resetMode ? args.Where(a => !string.Equals(a, "reset-today", StringComparison.OrdinalIgnoreCase)).ToArray() : args;

var builder = WebApplication.CreateBuilder(filteredArgs);

builder.Configuration.AddEnvironmentVariables(prefix: "GOALTRACKER_");
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
             builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
});
builder.Services.AddDatabaseMigrations();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<DashboardQueries>();
builder.Services.AddScoped<GoalCommands>();
builder.Services.AddScoped<MoodCommands>();
builder.Services.AddScoped<SeedData>();

var dbPathSetting = Environment.GetEnvironmentVariable("DB_FILE")
                    ?? builder.Configuration.GetValue<string>("Database:FilePath")
                    ?? "data/goaltracker.db";
var absoluteDbPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, dbPathSetting));
var dbDirectory = Path.GetDirectoryName(absoluteDbPath);
if (!string.IsNullOrEmpty(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}
var sqliteConnectionString = new SqliteConnectionStringBuilder
{
    DataSource = absoluteDbPath,
    ForeignKeys = true
}.ToString();
builder.Services.AddSingleton(new DatabaseConfig(sqliteConnectionString, absoluteDbPath));

var app = builder.Build();

await app.ApplyMigrationsAsync();

if (resetMode)
{
    await ResetTodayAsync(app.Services);
    return;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.MapGet("/", () => $"Hi this is GoalTracker API backend.");
app.MapControllers();

await app.RunAsync();

static async Task ResetTodayAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DapperContext>();
    await using var connection = context.CreateConnection();
    const string sql =
        """
        DELETE FROM Goals WHERE work_date < date('now');
        DELETE FROM MoodEntries WHERE work_date < date('now');
        VACUUM;
        """;
    await connection.ExecuteAsync(sql);

    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("ResetToday");
    logger.LogInformation("🧹 Removed previous-day rows.");
}
