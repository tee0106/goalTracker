using GoalTracker.Api.Data;
using GoalTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoalTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardQueries _queries;

    public DashboardController(DashboardQueries queries) => _queries = queries;

    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        var response = await _queries.GetDashboardAsync();
        return Ok(response);
    }
}


