using GoalTracker.Api.Data;
using GoalTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MoodController : ControllerBase
{
    private readonly MoodCommands _commands;

    public MoodController(MoodCommands commands) => _commands = commands;

    [HttpPost]
    [ProducesResponseType(typeof(MoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] UpdateMoodRequest request)
    {
        var result = await _commands.UpdateMoodAsync(request);
        return result is null ? BadRequest() : Ok(result);
    }
}


