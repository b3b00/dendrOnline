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
app.Run();