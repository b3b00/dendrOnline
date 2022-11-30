using System;
using System.IO;
using BackEnd;
using dendrOnline;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// builder.Services.AddRazorPages();

// this is to make demos easier
// don't do this in production
builder.Services.AddRazorPages(o => {
    //o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
 });

builder.Services.AddScoped<INotesService>((provider) =>
    new GithubNotesService());

builder.Services.AddSession().AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapRazorPages();
app.UseAuthorization();
app.UseAuthentication();
app.UseSession();


app.Use(async (context, next) =>
{
    var initialBody = context.Request.Body;

    using (var bodyReader = new StreamReader(context.Request.Body))
    {
        var forgeryService = context.RequestServices.GetService<IAntiforgery>();
        var forge = forgeryService?.GetAndStoreTokens(context);
        context.Response.Headers[forge.HeaderName] = forge.RequestToken;
        await next.Invoke();
        context.Request.Body = initialBody;
        
    }
});

app.UseGHOAuth(options =>
{
    options.TokenEndpoint = app.Configuration[Constants.TokenUrlParameter];
    options.AuthorizationEndpoint = app.Configuration[Constants.AuthorizeUrlParameter];
    options.ClientId = app.Configuration[Constants.ClientIdParameter];
    options.ClientSecret = app.Configuration[Constants.ClientSecretParameter];
    options.ReturnUrlParameter = app.Configuration[Constants.StartUrlParameter];
    options.RedirectUri = app.Configuration[Constants.RedirectUrlParameter];
});

if (app.Environment.IsDevelopment())
{
    app.Run();
}
else
{
    var port = Environment.GetEnvironmentVariable("PORT");
    Console.WriteLine("DendrOnline is listening to http://*:" + port);
    app.Run("http://*:" + port);
}