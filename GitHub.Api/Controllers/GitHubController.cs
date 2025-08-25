using Microsoft.AspNetCore.Mvc;
using Octokit;
using Servieces;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubService _gitHubService;

    public GitHubController(IGitHubService gitHubService)
    {
        _gitHubService = gitHubService;
    }

    [HttpGet("portfolio")]
    public async Task<IActionResult> GetPortfolio()
    {
        var portfolio = await _gitHubService.GetPortfolio();
        return Ok(portfolio);
    }


    [HttpGet("search")]
    public async Task<IActionResult> SearchRepositories( string name = null, string language = null,  string username =null )
    {
        var repositories = await _gitHubService.SearchRepositories(name, language, username);
        return Ok(repositories);
    }
}
