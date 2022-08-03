using BackEnd;
using GitHubOAuthMiddleWare;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using Octokit;

namespace TreeMeX.Pages;

[ValidateAntiForgeryToken]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string PostContent { get; set; }  = "*empty*";

    public List<string> Notes { get; set; } = new List<string>();
    
    public bool UpdateEditor { get; set; }
    
    public INoteHierarchy NoteHierarchy { get; set; }
    
    [BindProperty]
    [HiddenInput] 
    public string CurrentNote { get; set; }
    
    public User GitHubUser { get; set; }

    public string RepositoryId => HttpContext.Session.GetString("repositoryId");
    public string RepositoryName => HttpContext.Session.GetString("repositoryName");
    

    private INotesService NotesService => HttpContext.RequestServices.GetService<INotesService>();
    
    
    

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        
    }


    private async Task UpdateNotes()
    {
        if (HttpContext.HasRepository())
        {
            NotesService.SetRepository(HttpContext.GetRepositoryName(),HttpContext.GetRepositoryId());
            NotesService.SetAccessToken(HttpContext.GetGithubAccessToken());
        }
        
        Notes = await NotesService.GetNotes();
        NoteHierarchy = NotesService.GetHierarchy(Notes);
    }
    
    public async Task<IActionResult> OnPostSave(string PostContent, string CurrentNote)
    {
        NotesService.SetContent(CurrentNote,PostContent);
        await UpdateNotes();
        UpdateEditor = false;
        return Page();
    }
    
    public async Task<IActionResult> OnGet()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        this.GitHubUser = user;
        
        await UpdateNotes();
        UpdateEditor = false;
        if (!Request.IsHtmx())
        {
            CurrentNote = "root";
            PostContent = await NotesService.GetContent("root");
            return Page();
        }
        
        
        StringValues note = default;
        if (Request.Query.TryGetValue("note", out note))
        {
            CurrentNote = note.First();
            PostContent = await NotesService.GetContent(note.First());
            UpdateEditor = true;
        }
        
        return Partial("_Preview", this);
    }

    

    public IActionResult OnPost(string PostContent)
    {
        var notesService = HttpContext.RequestServices.GetService<INotesService>();
        if (!Request.IsHtmx())
        {
            return Page();
        }

        var content = PostContent; // Request.Query["PostContent"].First();
        this.PostContent = content;
        return Partial("_Preview", this);
    }

   
}