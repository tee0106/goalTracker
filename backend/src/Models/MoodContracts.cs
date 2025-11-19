using System.ComponentModel.DataAnnotations;

namespace GoalTracker.Api.Models;

public sealed class UpdateMoodRequest
{
    [Range(1, int.MaxValue)]
    public int MemberId { get; set; }

    [Required, RegularExpression("😀|😊|😐|😞|😤")]
    public string Emoji { get; set; } = "😀";
}

public sealed record MoodResponse(int MemberId, string Emoji, DashboardStats Stats);


