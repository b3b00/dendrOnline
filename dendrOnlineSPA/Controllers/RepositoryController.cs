using System.Text.Json;
using BackEnd;
using dendrOnlineSPA.Model;
using dendrOnlinSPA.model;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using System.Collections.Generic;

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
            var favorite = HttpContext.GetFavorite();
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
        var favorite = HttpContext.GetFavorite(); 
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
            var user = await client.User.Current();
            dendron.TheResult.RepositoryOwner = user.Login;
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

    [HttpPost("/image/{repositoryId}")]
    public async Task<Result<string>> AddImage(long repositoryId, IFormFile image)
    {
        var name = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string extension = ".png";
        if (image.ContentType == "image/jpeg")
        {
            extension = ".jpg";
        }

        if (image.ContentType == "image/png")
        {
            extension = ".png";
        }

        if (image.ContentType == "image/bmp")
        {
            extension = ".bmp";
        }
        var fileName = name+extension;
        
        Logger.LogInformation($"received file {name} withe fileName {fileName} of type {image.ContentType}");
        try
        {
            await NotesService.AddImage(image, fileName);
            
            return fileName;
        }
        catch (Exception ex)
        {
            return Result<string>.Error(ResultCode.InternalError, ex.Message);
        }

    }

    [HttpGet("/images/{repositoryId}")]
    public async Task<Result<IList<ImageAsset>>> GetImages(long repositoryId)
    {
        var result =  await NotesService.GetImages(repositoryId);
        return Result<IList<ImageAsset>>.Ok(result.TheResult);
    }
    
    
    public async Task<Result<IList<GhRepository>>> GetRepositories(GitHubClient client)
    {
        string repositoryList = HttpContext?.Session?.GetString("repositories");
        if (string.IsNullOrEmpty(repositoryList))
        {
            var repos = await client.Repository.GetAllForCurrent();
            var user = await client.User.Current();
            var owner = user.Login;
            var repositories = repos.Select(x => new GhRepository(x.Id, x.Name,owner)).ToList();
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