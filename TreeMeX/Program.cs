using System.Diagnostics;
using System.Text;
using BackEnd;
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

builder.Services.AddScoped<INotesService>((IServiceProvider provider) =>
    new FsNotesService(@"C:\Users\olduh\DendronNotes"));

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

app.Use(async (context, next) =>
{
    if (context.Session.TryGetValue("accessToken", out var accessToken))
    {
        
    }
    else
    {
        if (context.Request.Path == "/auth")
        {
            // TODO test if coming back from github => get the token => store it in session => redirect to /    
            HttpClient client = new HttpClient();
            var code = context.Request.Query["code"].First();
            var state = context.Request.Query["state"].First();
            var tokenUrl = builder.Configuration["github:tokenUrl"];
            var clientId = builder.Configuration["github:clientId"];
            var clientSecret = builder.Configuration["github:clientSecret"];
            var csrf = context.Session.GetString("CSRF:State");
            if (csrf != state)
            {
                context.Response.Redirect("/");
            }
            var query =
                $"client_id={clientId}&client_secret={clientSecret}&code={code}&redirect_uri=https://localhost:5003/auth";
            var token = await client.PostAsync(tokenUrl, new StringContent(query, Encoding.UTF8, "application/x-www-form-urlencoded"));
            var body = await token.Content.ReadAsStringAsync();
            Debug.WriteLine(body);
            // TODO split body to token nd authorization scheme
        }
        else
        {
            var authUrl = builder.Configuration["github:authorizeUrl"];
            var clientId = builder.Configuration["github:clientId"];
            string csrf = "Membership.GeneratePassword(24, 1)";
            authUrl =
                $"{authUrl}?redirect_uri=https://localhost:5003/auth&response_type=code&client_id={clientId}&scope=repo&state={csrf}";

            context.Session.SetString("CSRF:State", csrf);
            context.Response.Redirect(authUrl);
        }
    }
    await next.Invoke();
});
app.Run();