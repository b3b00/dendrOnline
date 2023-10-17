using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHubOAuthMiddleWare;
using Htmx;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Octokit;
using TreeMeX.Pages;

namespace dendrOnline.Pages;

[ValidateAntiForgeryToken]
public class LogoutModel : BaseModel
{
    private readonly IConfiguration _configuration;

    public LogoutModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    

    public async Task<IActionResult> OnGet()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        await client.Logout(HttpContext, HttpContext.GetGithubAccessToken(), _configuration[Constants.ClientIdParameter],_configuration[Constants.LogoutUrlParameter]);
        HttpContext.Session.Clear();
        return Redirect("/Index");
    }
    
}