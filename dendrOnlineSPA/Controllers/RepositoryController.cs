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

    [HttpGet("/notes/{repositoryId}")]
    public async Task<INoteHierarchy> GetNotesHierarchy(long repositoryId)
    {
        HttpContext.SetRepositoryId(repositoryId);
        var notes = await NotesService.GetNotes();
        var hierarchy = NotesService.GetHierarchy(notes, null, HttpContext.GetCurrentNote(), HttpContext.GetEditedNotes().Keys.ToList());
        var json = JsonSerializer.Serialize(hierarchy);
        return hierarchy;
    }

    [HttpGet("/note/{repositoryId}/{noteId}")]
    public async Task<Note> GetNote(string repositoryId, string noteId)
    {
        var note = await NotesService.GetNote(noteId);
        return note;
    }


    [HttpPut("/note/{repositoryId}/{noteId}")]
    public async Task<INoteHierarchy> SaveNote(string repositoryId,string noteId, [FromBody] Note note)
    {
        await NotesService.SetContent(noteId, note.ToString());
        var tree = await GetNotesHierarchy(long.Parse(repositoryId));
        return tree;
        //return await GetNote(repositoryId, note.Header.Title);
    }
    
    [HttpDelete("/note/{repositoryId}/{noteId}")]
    public async Task<INoteHierarchy> DeleteNote(string repositoryId,string noteId, [FromQuery] bool recurse = false)
    {
        if (recurse)
        {
            var allNotes = await NotesService.GetNotes();
            var children = allNotes.Where(x => x.StartsWith(noteId) && x != noteId);
            foreach (var child in children)
            {
                await NotesService.DeleteNote(child);
            }
        }
        await NotesService.DeleteNote(noteId);
        
        var tree = await GetNotesHierarchy(long.Parse(repositoryId));
        return tree;
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