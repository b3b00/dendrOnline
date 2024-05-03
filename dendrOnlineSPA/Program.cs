using System.Reflection.Metadata;
using BackEnd;
using dendrOnlineSPA;
using GitHubOAuthMiddleWare;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(1); });
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors",
        policy => { policy.WithOrigins("*"); });
});
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "dendrOnline.Session";
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.IsEssential = true;
    
});





builder.Services.AddScoped<INotesService, GithubNotesService>();
builder.Services.AddSingleton<IMongoService,MongoService>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseSession();
app.UseStaticFiles();
app.UseCors();
app.UseGHOAuth(options =>
{
    options.TokenEndpoint = app.Configuration[Constants.TokenUrlParameter];

    options.AuthorizationEndpoint = app.Configuration[Constants.AuthorizeUrlParameter];
    options.ClientId = app.Configuration[Constants.ClientIdParameter];
    options.ClientSecret = app.Configuration[Constants.ClientSecretParameter];
    options.ReturnUrlParameter = app.Configuration[Constants.StartUrlParameter];
    options.RedirectUri = app.Configuration[Constants.RedirectUrlParameter];
    options.ExcludePath = "/health";
});

app.UseNoCors();

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