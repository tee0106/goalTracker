using GoalTracker.Api.Data;
using GoalTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GoalsController : ControllerBase
{
    private readonly GoalCommands _commands;

    public GoalsController(GoalCommands commands) => _commands = commands;

    [HttpPost]
    [ProducesResponseType(typeof(GoalResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] AddGoalRequest request)
    {
        var result = await _commands.AddGoalAsync(request);
        return result is null ? BadRequest() : Created("/api/goals", result);
    }

    [HttpPatch("{goalId:int}")]
    [ProducesResponseType(typeof(GoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleAsync([FromRoute] int goalId, [FromBody] ToggleGoalRequest request)
    {
        var result = await _commands.ToggleGoalAsync(goalId, request.IsCompleted);
        return result is null ? NotFound() : Ok(result);
    }
}


