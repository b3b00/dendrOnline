using BackEnd;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using dendrOnlineSPA.Model;
using dendrOnlinSPA.model;
using Octokit;

namespace dendrOnlineSPA.Controllers;

public class FavoriteController : ControllerBase
{
    
    private readonly ILogger<FavoriteController> _logger;

    protected ILogger<FavoriteController> Logger => _logger;

    private readonly IConfiguration _configuration;

    protected IConfiguration Configuration => _configuration;

    private readonly IMongoService _mongoService;

    protected IMongoService MongoService => _mongoService;

    public FavoriteController(ILogger<FavoriteController> logger, IConfiguration configuration, IMongoService mongoService)
    {
        _logger = logger;
        _configuration = configuration;
        _mongoService = mongoService;
    }

    [HttpPost("/favorite/{repositoryId}")]
    public async Task<IActionResult> SetFavorite(long repositoryId)
    {
        MongoService.SaveFavorite(HttpContext.GetUserId(), repositoryId);
        return Ok();
    }

    [HttpGet("/favorite")]
    public async Task<Result<Favorite>> GetFavorite()
    {
        
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var currentUser = await client.User.Current();
        var userId = currentUser.Id;
        
        Logger.LogInformation($"looking for favorite for user {userId} {currentUser.Name} {currentUser.Login}");
        var favorite = await MongoService.GetFavorite(userId);
        
        
        
        if (favorite != null)
        {
            Logger.LogInformation($"found a favorite repo {favorite.Repository}");
            client.Credentials = new Credentials(accessToken);
            var repo = await client.Repository.Get(favorite.Repository);
            Logger.LogInformation($"favortie repo name is {repo.Name}");
            favorite.RepositoryName = repo.Name;
            return favorite;
        }
        else
        {
            Logger.LogInformation($"no favorite repo found");
            return Result<Favorite>.Error(ResultCode.NotFound,ConflictCode.NoConflict,"no favorite repository");
            
            
        }
    }

    

}