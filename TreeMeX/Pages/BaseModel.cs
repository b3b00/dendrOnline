using BackEnd;
using dendrOnline;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace TreeMeX.Pages;

public class BaseModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string NoteName { get; set; }
    
    public User GitHubUser { get; set; }
    
    public string RepositoryName { get; set; }
    
    public long RepositoryId { get; set; }
    
    public string CurrentNoteDescription { get; set; }

    public bool IsRepositoryLoaded => !string.IsNullOrEmpty(RepositoryName);

    protected INotesService NotesService => HttpContext.RequestServices.GetService<INotesService>();

    protected void SetClient()
    {
        if (HttpContext.HasRepository())
        {
            NotesService.SetRepository(HttpContext.GetRepositoryName(),HttpContext.GetRepositoryId());
            NotesService.SetUser(HttpContext.GetUserName(),HttpContext.GetUserId());
            NotesService.SetAccessToken(HttpContext.GetGithubAccessToken());
        }
    }

    protected void SetRepository()
    {
        RepositoryName = HttpContext.GetRepositoryName();
        RepositoryId = HttpContext.GetRepositoryId();
    }
}