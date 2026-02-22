using Infrastructure.Queries;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly StandingQueryRepository _standingRepository;
    private readonly TopScorerQueryRepository _topScorerRepository;

    public StatsController(
        StandingQueryRepository standingRepository,
        TopScorerQueryRepository topScorerRepository)
    {
        _standingRepository = standingRepository;
        _topScorerRepository = topScorerRepository;
    }

    [HttpGet("standings")]
    public async Task<IActionResult> GetStandings(
    int pageNumber = 1,
    int pageSize = 10,
    string? sortBy = "points",
    string? sortDirection = "desc")
    {
        var result = await _standingRepository
            .GetStandingsAsync(pageNumber, pageSize, sortBy, sortDirection);

        return Ok(result);
    }

    [HttpGet("topscorers")]
    public async Task<IActionResult> GetTopScorers(
    int pageNumber = 1,
    int pageSize = 10,
    string? sortBy = "goals",
    string? sortDirection = "desc",
    Guid? teamId = null)
    {
        var result = await _topScorerRepository
            .GetTopScorersAsync(pageNumber, pageSize, sortBy, sortDirection, teamId);

        return Ok(result);
    }
}