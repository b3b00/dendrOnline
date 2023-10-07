using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace GitHubOAuthMiddleWare;

public static class GitHubOAuthMiddleware
{
    
    public static string AccessToken = "accessToken";
        
    public static string TokenType = "tokenType";

    public class GitHubOptions : OAuthOptions
    {
        public string RedirectUri { get; set; }

        public override string ToString() {
            return $@"
            redirectUri:{RedirectUri}
            tokenUrl:{TokenEndpoint}
            AuthorizationEndpoint:{AuthorizationEndpoint}
            ClientId:{ClientId}
            ClientSecret:{ClientSecret}
            ReturnUrlParameter:{ReturnUrlParameter}
            ";
        }
    }

    public static WebApplication UseGHOAuth(this WebApplication app, Action<GitHubOptions> configuration)
    {
        GitHubOptions options = new GitHubOptions();
        configuration(options);
        Console.WriteLine(options);
        app.Use(async (context, next) =>
        {
            Console.WriteLine(options);
            bool existAccessToken = context.Session.TryGetValue(AccessToken, out var accessToken);
            Console.WriteLine($"existing token ? {existAccessToken}");
            if (!existAccessToken || accessToken == null || accessToken.Length == 0)
            {
                Console.WriteLine($"no access token @ {context.Request.HostAndPath()}");
                if (context.Request.HostAndPath() == options.RedirectUri.Replace("https://","").Replace("http://",""))
                {
                    Console.WriteLine($"{context.Request.HostAndPath()} is redircet uri {options.RedirectUri}");
                    // check if error in query string
                    // test if coming back from github => get the token => store it in session => redirect to /    
                    HttpClient client = new HttpClient();
                    var code = context.Request.Query["code"].First();
                    var state = context.Request.Query["state"].First();
                    var tokenUrl = options.TokenEndpoint;
                    var clientId = options.ClientId;
                    var clientSecret = options.ClientSecret;
                    var redirectUri = options.RedirectUri;
                    var csrf = context.Session.GetString("CSRF:State");
                    
                    Console.WriteLine($"testing CSRF : {csrf}//{state}");
                    
                    if (csrf != state)
                    {
                        throw new InvalidOperationException("authentication failure : CSRF suspected.");
                    }
                    var query =
                        $"client_id={clientId}&client_secret={clientSecret}&code={code}&redirect_uri={redirectUri}";
                    Console.WriteLine($"getting access token @ {query}");
                    
                    var response = await client.PostAsync(tokenUrl,
                        new StringContent(query, Encoding.UTF8, "application/x-www-form-urlencoded"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("access toke nrequest OK");
                        var body = await response.Content.ReadAsStringAsync();
                        var parameters = QueryHelpers.ParseQuery(body);
                        Console.WriteLine($"access token response {parameters}");
                        if (parameters.TryGetValue("access_token", out var accessTokenValues))
                        {
                            Console.WriteLine($"token = [{accessTokenValues.First()}");
                            context.Session.SetString(AccessToken, accessTokenValues.First());
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"invalid authentication : missing access token");
                        }
                        
                        if (parameters.TryGetValue("token_type", out var tokenTypeValues))
                        {
                            context.Session.SetString(TokenType, tokenTypeValues.First());
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"inavlid authentication : missing token type");
                        }
                        Console.WriteLine($"redirecting to {options.ReturnUrlParameter}");
                        context.Response.Redirect(options.ReturnUrlParameter);
                        return;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"invalid authentication : {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                else
                {
                    var authUrl = options.AuthorizationEndpoint;
                    var clientId = options.ClientId;
                    var redirectUri = options.RedirectUri;
                    string csrf = GenerateStatePassword(48);
                    authUrl =
                        $"{authUrl}?redirect_uri={redirectUri}&response_type=code&client_id={clientId}&scope=repo&state={csrf}";
                    Console.WriteLine($"no token : redirecting to {authUrl}");    
                    context.Session.SetString("CSRF:State", csrf);
                    context.Response.Redirect(authUrl);
                    return;
                }
            }

                await next.Invoke();
        });
        return app;
    }

    private static string GenerateStatePassword(int len)
    {

    string choices = "abcdefghijklmnopqrstuvwxyz012345679ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    StringBuilder state = new StringBuilder();
    Random rnd = new Random();
    for (int i = 0; i < len; i++)
    {
        var x = rnd.Next(choices.Length);
        state.Append(choices[x]);
    }
    return state.ToString();
    }

}
