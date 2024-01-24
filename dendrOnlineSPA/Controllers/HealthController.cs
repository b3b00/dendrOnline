using BackEnd;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using dendrOnlineSPA.Model;
using dendrOnlinSPA.model;

namespace dendrOnlineSPA.Controllers;

public class HealthController : ControllerBase
{
    
    
    
    private readonly ILogger<HealthController> _logger;

    protected ILogger<HealthController> Logger => _logger;

    private readonly IConfiguration _configuration;

    protected IConfiguration Configuration => _configuration;

    

    public HealthController(ILogger<HealthController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration; }

    [HttpGet("/health")]
    public async Task<IActionResult> Get()
    {
        return Ok("OK");
    }

}