## configuration


```csharp
app.UseGHOAuth(options =>
{
    options.TokenEndpoint = app.Configuration["github:tokenUrl"];
    options.AuthorizationEndpoint = app.Configuration["github:authorizeUrl"];
    options.ClientId = app.Configuration["github:clientId"];
    options.ClientSecret = app.Configuration["github:clientSecret"];
    options.ReturnUrlParameter = app.Configuration["github:startUri"];
    options.RedirectUri = app.Configuration["github:redirectUri"];
});
```

## get access token

access token and token type are stored in session :
 * ```accessToken``` : for access token
 * ```tokenType``` : for token type (bearer)