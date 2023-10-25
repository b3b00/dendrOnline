using System.Text.Json;
using BackEnd;
using dendrOnlineSPA.Model;
using dendrOnlinSPA.model;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace dendrOnlineSPA.Controllers;

[ApiController]
[Route("[controller]")]
public class RepositoryController : DendronController
{



    public RepositoryController(ILogger<RepositoryController> logger, IConfiguration configuration,
        INotesService notesService) : base(logger, configuration, notesService)
    {
    }

    [HttpGet("/Index")]
    public async Task Index()
    {
        HttpContext.Response.Redirect("/index.html");
        return;
    }

    [HttpGet("/user")]
    public async Task<GHUser> User()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        return new GHUser(user);
    }
    
    [HttpGet("/repositories")]
    public async Task<IEnumerable<GhRepository>> Get()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        return await GetRepositories(client);
    }

    [HttpGet("/notes/{repositoryId}/{filter?}")]
    public async Task<INoteHierarchy> GetNotesHierarchy(long repositoryId,string filter)
    {
        HttpContext.SetRepositoryId(repositoryId);
        var notes = await NotesService.GetNotes();
        var hierarchy = NotesService.GetHierarchy(notes, filter, HttpContext.GetCurrentNote(), HttpContext.GetEditedNotes().Keys.ToList());
        var json = JsonSerializer.Serialize(hierarchy);
        return hierarchy;
    }
    
    public async Task<IList<GhRepository>> GetRepositories(GitHubClient client)
    {
        string repositoryList = HttpContext?.Session?.GetString("repositories");
        if (string.IsNullOrEmpty(repositoryList))
        {
            var repos = await client.Repository.GetAllForCurrent();
            var repositories = repos.Select(x => new GhRepository(x.Id, x.Name)).ToList();
            repositoryList = JsonSerializer.Serialize(repositories);
            HttpContext?.Session?.SetString("repositories", repositoryList);
            return repositories;
        }
        else
        {
            var repositories = JsonSerializer.Deserialize<IList<GhRepository>>(repositoryList) ??
                           new List<GhRepository>();
            return repositories;
        }
    }
}