namespace GoalTracker.Api.Models;

public sealed record GoalDto(int Id, string Description, bool IsCompleted);

public sealed record MemberDto(
    int Id,
    string Name,
    string Emoji,
    IReadOnlyList<GoalDto> Goals,
    int CompletedCount,
    int TotalCount,
    string HelperText);

public sealed record MoodDto(int MemberId, string Emoji);

public sealed record DashboardStats(double CompletionPercent, int HappyCount, int NeutralCount, int StressedCount);

public sealed record DashboardResponse(IReadOnlyList<MemberDto> Members, DashboardStats Stats);


