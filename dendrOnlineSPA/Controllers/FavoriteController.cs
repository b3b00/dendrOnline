using BackEnd;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using Octokit;


namespace dendrOnlineSPA.Controllers;

public class FavoriteController : DendronController
{

    public FavoriteController(ILogger<FavoriteController> logger, IConfiguration configuration,
        INotesService notesService) : base(logger, configuration, notesService)
    {
        
    }

    [HttpPost("/favorite/{repositoryId}")]
    public async Task<IActionResult> SetFavorite(long repositoryId)
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var repo = await client.Repository.Get(repositoryId);
        HttpContext.SetFavorite(repositoryId, repo.Name);
        return Ok();
    }

    [HttpGet("/favorite")]
    public async Task<Result<Favorite>> GetFavorite()
    {

        var fav = HttpContext.GetFavorite();
        if (fav != null)
        {
            return Result<Favorite>.Ok(fav);
        }
        else
        {
            Logger.LogInformation($"no favorite repo found");
            return Result<Favorite>.Error(ResultCode.NotFound,ConflictCode.NoConflict,"no favorite repository");
            
            
        }
    }

    

}