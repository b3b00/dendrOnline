using BackEnd;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using dendrOnlineSPA.Model;
using dendrOnlinSPA.model;
using Octokit;

namespace dendrOnlineSPA.Controllers;

public class DendronController : ControllerBase
{
    
    
    
    private readonly ILogger<DendronController> _logger;

    protected ILogger<DendronController> Logger => _logger;

    private readonly IConfiguration _configuration;

    protected IConfiguration Configuration => _configuration;

    private readonly INotesService _notesService;

    protected INotesService NotesService
    {
        get
        {
            SetClient();
            return _notesService;
        }
    }

    public DendronController(ILogger<DendronController> logger, IConfiguration configuration, INotesService notesService)
    {
        _logger = logger;
        _configuration = configuration;
        _notesService = notesService;
    }
    
    protected void SetClient()
    {
        if (HttpContext.HasRepository())
        {
            _notesService.SetRepository(HttpContext.GetRepositoryName(),HttpContext.GetRepositoryId());
        }
        _notesService.SetUser(HttpContext.GetUserName(),HttpContext.GetUserId());
        _notesService.SetAccessToken(HttpContext.GetGithubAccessToken());
    }
    
}