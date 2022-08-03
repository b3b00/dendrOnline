using System.Diagnostics;
using System.Text;
using BackEnd;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddRazorPages();

// this is to make demos easier
// don't do this in production
builder.Services.AddRazorPages(o => {
    
     o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
 });

// builder.Services.AddScoped<INotesService>((IServiceProvider provider) =>
//     new FsNotesService(@"C:\Users\olduh\DendronNotes"));

builder.Services.AddScoped<INotesService>((IServiceProvider provider) =>
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
    options.TokenEndpoint = app.Configuration["github:tokenUrl"];
    options.AuthorizationEndpoint = app.Configuration["github:authorizeUrl"];
    options.ClientId = app.Configuration["github:clientId"];
    options.ClientSecret = app.Configuration["github:clientSecret"];
    options.ReturnUrlParameter = app.Configuration["github:startUri"];
    options.RedirectUri = app.Configuration["github:redirectUri"];
});
app.Run();