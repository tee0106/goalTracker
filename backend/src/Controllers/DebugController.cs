using GoalTracker.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Api.Controllers;

[ApiController]
[Route("api/debug")]
public sealed class DebugController : ControllerBase
{
    private readonly SeedData _seedData;

    public DebugController(SeedData seedData) => _seedData = seedData;

    [HttpPost("seed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SeedAsync(CancellationToken cancellationToken)
    {
        var inserted = await _seedData.SeedAsync(cancellationToken);
        return Ok(new { inserted });
    }
}


