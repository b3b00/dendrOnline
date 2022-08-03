﻿using BackEnd;
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


    private INotesService NotesService => HttpContext.RequestServices.GetService<INotesService>();
    
    
    

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        
    }


    private void UpdateNotes()
    {
        Notes = NotesService.GetNotes();
        NoteHierarchy = NotesService.GetHierarchy(Notes);
    }
    
    public IActionResult OnPostSave(string PostContent, string CurrentNote)
    {
        NotesService.SetContent(CurrentNote,PostContent);
        UpdateNotes();
        UpdateEditor = false;
        return Page();
    }
    
    public async Task<IActionResult> OnGet()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.Session.GetString(GitHubOAuthMiddleWare.GitHubOAuthMiddleware.AccessToken);
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        this.GitHubUser = user;
        
        UpdateNotes();
        UpdateEditor = false;
        if (!Request.IsHtmx())
        {
            CurrentNote = "root";
            PostContent = NotesService.GetContent("root");
            return Page();
        }
        
        
        StringValues note = default;
        if (Request.Query.TryGetValue("note", out note))
        {
            CurrentNote = note.First();
            PostContent = NotesService.GetContent(note.First());
            UpdateEditor = true;
        }
        
        return Partial("_Preview", this);
    }

    public User GitHubUser { get; set; }

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