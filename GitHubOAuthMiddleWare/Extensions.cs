using Microsoft.AspNetCore.Http;
using Octokit;

namespace GitHubOAuthMiddleWare;

public static class Extensions
{
    public static string GetGithubAccessToken(this HttpContext httpContext)
    {
        return httpContext.Session.GetString(GitHubOAuthMiddleWare.GitHubOAuthMiddleware.AccessToken);
    }
    
    public static void DeleteGithubAccessToken(this HttpContext httpContext)
    {
        httpContext.Session.Remove(GitHubOAuthMiddleWare.GitHubOAuthMiddleware.AccessToken);
        var cookies = httpContext.Request.Cookies.Keys;
    }

    public static void SetGithubAccessToken(this HttpContext httpContext, string value)
    {
        httpContext.Session.SetString(GitHubOAuthMiddleWare.GitHubOAuthMiddleware.AccessToken, value);
    }
    
    public static string GetGithubTokenType(this HttpContext httpContext)
    {
        return httpContext.Session.GetString(GitHubOAuthMiddleWare.GitHubOAuthMiddleware.TokenType);
    }

    public async static Task Logout(this GitHubClient gitHubClient, HttpContext context, string accessToken, string clientId, string logoutUrl)
    {
        HttpClient client = new HttpClient();
        context.DeleteGithubAccessToken();
        context.Session.Clear();
        
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{logoutUrl}{clientId}/grant");
        FormUrlEncodedContent content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            { new KeyValuePair<string, string>("access_token", accessToken) });
        request.Content = content;
        request.Headers.Add("User-Agent", "Awesome-Octocat-App");
        request.Headers.Add("Authorization",$"Bearer {accessToken}");
        var r = await client.SendAsync(request);
        var status = r.StatusCode;
        var reason = r.ReasonPhrase;
        var contentR = new StreamReader(r.Content.ReadAsStream()).ReadToEnd();
        ;
    }

    public static string Url(this HttpRequest request) => request.Scheme + "://" + request.Host + request.Path;
    
    
}