using System.ComponentModel.DataAnnotations;

namespace GoalTracker.Api.Models;

public sealed class AddGoalRequest
{
    [Range(1, int.MaxValue)]
    public int MemberId { get; set; }

    [Required, StringLength(250, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;
}

public sealed class ToggleGoalRequest
{
    public bool IsCompleted { get; set; }
}

public sealed record GoalResponse(MemberDto Member, DashboardStats Stats);


