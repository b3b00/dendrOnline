using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd;
using GitHubOAuthMiddleWare;
using Htmx;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Octokit;

namespace dendrOnline.Pages;

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


    private void SetClient()
    {
        if (HttpContext.HasRepository())
        {
            NotesService.SetRepository(HttpContext.GetRepositoryName(),HttpContext.GetRepositoryId());
            NotesService.SetAccessToken(HttpContext.GetGithubAccessToken());
        }
    }

    private async Task UpdateNotes()
    {
        SetClient();
        
        Notes = await NotesService.GetNotes();
        NoteHierarchy = NotesService.GetHierarchy(Notes);
        NoteHierarchy.Deploy(CurrentNote);
    }
    
    public async Task<IActionResult> OnPostSave(string PostContent, string CurrentNote)
    {
        SetClient();
        NotesService.SetContent(CurrentNote,PostContent);
        
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;
        this.PostContent = PostContent;
        await UpdateNotes();
        UpdateEditor = false;
        return Page();
    }

   


    public async Task<IActionResult> OnGetNewNote()
    {
        SetClient();
        string parent = Request.Query["parent"].First();
        string newNote = Request.Query["new"].First();
        await NotesService.CreateNote(newNote);
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        this.GitHubUser = user;
        CurrentNote = newNote;
        
        await UpdateNotes();
        
        CurrentNote = "root";
        PostContent = await NotesService.GetContent(newNote);
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

    

    public async Task<IActionResult> OnPost(string PostContent)
    {
        SetClient();
        var notesService = HttpContext.RequestServices.GetService<INotesService>();
        if (!Request.IsHtmx())
        {
            return Page();
        }
        
        var content = PostContent; // Request.Query["PostContent"].First();
        this.PostContent = content;
        
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;

        UpdateNotes();
        
        return Partial("_Preview", this);
    }

   
}