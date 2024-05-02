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

    private IMongoService _mongoService;

    public RepositoryController(ILogger<RepositoryController> logger, IConfiguration configuration,
        INotesService notesService, IMongoService mongoService) : base(logger, configuration, notesService)
    {
        _mongoService = mongoService;
    }

    [HttpGet("/Index")]
    public async Task Index()
    {
        HttpContext.Response.Redirect("/index.html");
    }

    [HttpGet("/user")]
    public async Task<Result<GHUser>> User()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        return new GHUser(user);
    }
    
    [HttpGet("/repositories")]
    public async Task<Result<IList<GhRepository>>> Get()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        return await GetRepositories(client);
    }

    [HttpGet("/notes/{repositoryId}")]
    public async Task<Result<INoteHierarchy>> GetNotesHierarchy(long repositoryId)
    {
        HttpContext.SetRepositoryId(repositoryId);
        var notes = await NotesService.GetNotes();
        var hierarchy = await NotesService.GetHierarchy(notes, null, HttpContext.GetCurrentNote(), HttpContext.GetEditedNotes().Keys.ToList());
        return hierarchy;
    }

    [HttpGet("/dendron/{repositoryId}")]
    public async Task<Result<Dendron>> GetDendron(long repositoryId)
    {
        
        HttpContext.SetRepositoryId(repositoryId);
        var dendron = await NotesService.GetDendron();
        if (dendron.IsOk)
        {
            dendron.TheResult.RepositoryId = repositoryId;
            var favorite = await _mongoService.GetFavorite(HttpContext.GetUserId());
            dendron.TheResult.IsFavoriteRepository = favorite != null && favorite.Repository == repositoryId;
        }
        return dendron;
    }
    
    [HttpGet("/favorite/dendron/")]
    public async Task<Result<Dendron>> GetFavoriteDendron()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var currentUser = await client.User.Current();
        var userId = currentUser.Id;
        var favorite = await _mongoService.GetFavorite(userId);
        HttpContext.Session.SetString("userId",userId.ToString());
        
        if (favorite != null)
        {
           
            var repository = await client.Repository.Get(favorite.Repository);
            Logger.LogInformation($"loading favorite repository {repository.Name} for user {currentUser.Name}");
            HttpContext.SetRepositoryId(favorite.Repository);
            var dendron = await NotesService.GetDendron();
            dendron.TheResult.RepositoryId = favorite.Repository;
            dendron.TheResult.IsFavoriteRepository = true;
            dendron.TheResult.RepositoryName = repository.Name;
            return dendron;
        }
        else
        {
            return Result<Dendron>.Error(ResultCode.NotFound, ConflictCode.NoConflict,
                $"no favorite repository found");
        }
    }

    [HttpGet("/note/{repositoryId}/{noteId}")]
    public async Task<Result<Note>> GetNote(string repositoryId, string noteId)
    {
        var note = await NotesService.GetNote(noteId);
        return note;
    }


    [HttpPut("/note/{repositoryId}/{noteId}")]
    public async Task<Result<HierarchyAndSha>> SaveNote(string repositoryId, string noteId, [FromBody] Note note)
    {
        var setted = await NotesService.SetContent(noteId, note);
        if (!setted.IsOk)
        {
            return Result<HierarchyAndSha>.TransformError<Note,HierarchyAndSha>(setted);
        }
        
        var tree = await GetNotesHierarchy(long.Parse(repositoryId));
        if (!tree.IsOk)
        {
            return Result<HierarchyAndSha>.TransformError<INoteHierarchy,HierarchyAndSha>(tree);
        }

        var treeAndSha = new HierarchyAndSha()
        {
            Hierarchy = tree.TheResult,
            Sha = setted.TheResult.Sha
        };
        
        return treeAndSha;
    }

    [HttpDelete("/note/{repositoryId}/{noteId}")]
    public async Task<Result<INoteHierarchy>> DeleteNote(string repositoryId,string noteId, [FromQuery] bool recurse = false)
    {
        if (recurse)
        {
            var allNotes = await NotesService.GetNotes();
            if (!allNotes.IsOk)
            {
                return Result<INoteHierarchy>.TransformError<List<string>,INoteHierarchy>(allNotes);
            }
            var children = allNotes.TheResult.Where(x => x.StartsWith(noteId) && x != noteId);
            foreach (var child in children)
            {
                await NotesService.DeleteNote(child);
            }
        }
        var deleted = await NotesService.DeleteNote(noteId);
        if (!deleted.IsOk)
        {
            return Result<INoteHierarchy>.TransformError<Note,INoteHierarchy>(deleted);
        }
        Logger.LogDebug($"after note deletion {repositoryId} - {noteId} - {recurse}");
        var tree = await GetNotesHierarchy(long.Parse(repositoryId));
        Logger.LogDebug($" new tree : {tree.IsOk} - {tree.ErrorMessage}");
        if (tree.TheResult != null)
        {
            Logger.LogDebug("new tree :: ");
            Logger.LogDebug(tree.TheResult.Dump("  "));
        }
        return tree;
    }
    
    
    
    public async Task<Result<IList<GhRepository>>> GetRepositories(GitHubClient client)
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
            var repositories = JsonSerializer.Deserialize<IList<GhRepository>>(repositoryList).ToList();
            return repositories;
        }
    }
}