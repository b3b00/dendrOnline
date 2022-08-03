using BackEnd;
using GitHubOAuthMiddleWare;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Octokit;
using Page = Octokit.Page;

namespace TreeMeX.Pages;

[ValidateAntiForgeryToken]
public class ChooseRepoModel : PageModel
{

    public User GitHubUser { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string Query { get; set; }
    
    public List<(long id,string name)> Repositories { get; set; }
    
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
                HttpContext.Session.SetString("repositoryId",Request.Query["repo"].First());
                var repo = await client.Repository.Get(long.Parse(Request.Query["repo"].First()));
                HttpContext.Session.SetString("repositoryName",repo.Name);
                Response.Redirect("/Index");
            }
            
            var repos = await client.Repository.GetAllForCurrent();
            Repositories = repos.Select(x => (x.Id, x.Name)).ToList();
            return Page();
        }
        else
        {
           
            var repos = (await client.Repository.GetAllForCurrent()).Where(x => x.Name.Contains(Query));
            Repositories = repos.Select(x => (x.Id, x.Name)).ToList();
            return Partial("RepositoryList",this);
        }
    }
    
}