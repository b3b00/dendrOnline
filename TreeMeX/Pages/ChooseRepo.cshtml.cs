using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHubOAuthMiddleWare;
using Htmx;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Octokit;

namespace dendrOnline.Pages;

public class GhRepository
{
    public long Id { get; set; }
    
    public string Name { get; set; }

    public GhRepository(long id, string name)
    {
        Id = id;
        Name = name;
    }


}


[ValidateAntiForgeryToken]
public class ChooseRepoModel : PageModel
{

    public User GitHubUser { get; set; }

    [BindProperty(SupportsGet = true)] public string Query { get; set; } = "";

    public List<GhRepository> Repositories { get; set; } = new List<GhRepository>();

    public async Task GetRepositories(GitHubClient client)
    {
        string repositoryList = HttpContext?.Session?.GetString("repositories");
        if (string.IsNullOrEmpty(repositoryList))
        {
            var repos = await client.Repository.GetAllForCurrent();
            this.Repositories = repos.Select(x => new GhRepository(x.Id, x.Name)).ToList();
            repositoryList = JsonConvert.SerializeObject(Repositories);
            HttpContext?.Session?.SetString("repositories", repositoryList);
        }
        else
        {
            Repositories = JsonConvert.DeserializeObject<List<GhRepository>>(repositoryList) ??
                           new List<GhRepository>();
        }
    }


    public async Task<IActionResult> OnGet()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        this.GitHubUser = user;

        if (!Request.IsHtmx())
        {
            if (Request.Query.ContainsKey("repo"))
            {
                HttpContext.Session.SetString("repositoryId", Request.Query["repo"].First());
                var repo = await client.Repository.Get(long.Parse(Request.Query["repo"].First()));
                HttpContext.Session.SetString("repositoryName", repo.Name);
                HttpContext.Session.SetInt32("userId", user.Id);
                HttpContext.Session.SetString("userName", user.Name);
                Response.Redirect("/Index");
            }

            await GetRepositories(client);


            return Page();
        }
        else
        {
            await GetRepositories(client);

            if (!string.IsNullOrEmpty(Query))
                Repositories = Repositories
                    .Where(x => x.Name.Contains(Query, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        return Partial("RepositoryList", this);
    }
}

    


