using Microsoft.AspNetCore.Http;

namespace GitHubOAuthMiddleWare;

public static class Extensions
{
    public static string GetGithubAccessToken(this HttpContext httpContext)
    {
        return httpContext.Session.GetString(GitHubOAuthMiddleWare.GitHubOAuthMiddleware.AccessToken);
    }
    
    public static string GetGithubTokenType(this HttpContext httpContext)
    {
        return httpContext.Session.GetString(GitHubOAuthMiddleWare.GitHubOAuthMiddleware.TokenType);
    }
}