
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackEnd;
using dendrOnline;
using dendrOnline.Pages;
using GitHubOAuthMiddleWare;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;

namespace TreeMeX.Pages;

public class TreeModel : BaseModel
{
    
    private readonly ILogger<IndexModel> _logger;
    
    [BindProperty(SupportsGet = true)]
    public string NoteName { get; set; }
    
    public string PostContent { get; set; }  = "*empty*";
    
    
    public async Task<IActionResult> OnGet()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;
        
        SetRepository();
        
        SetClient();
        var Notes = await NotesService.GetNotes();
        NoteHierarchy = NotesService.GetHierarchy(Notes,null,NoteName, HttpContext.GetEditedNotes()?.Keys?.ToList());
        NoteHierarchy.Deploy(CurrentNote);
        
        if (!string.IsNullOrEmpty(NoteName))
        {
            var editedNotes = HttpContext.GetEditedNotes();
            var content = "";
            if (editedNotes == null || !editedNotes.TryGetValue(NoteName, out content))
            {
                SetClient();
                content = await NotesService.GetContent(NoteName);
            }
            var note = NoteParser.Parse(content);
            CurrentNoteDescription = note.Header.TrimmedDescription;
            PostContent = content;
            return Page();            
        }
        else
        {
            return Redirect("/Tree");
        }
        
    }

    public async Task OnPost(string postContent)
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;
        
        RepositoryName = HttpContext.GetRepositoryName();
        RepositoryId = HttpContext.GetRepositoryId();
        
        SetClient();
        var notesService = HttpContext.RequestServices.GetService<INotesService>();

        var content = postContent; 
        PostContent = content;

        var editedNotes = HttpContext.GetEditedNotes();
        if (editedNotes == null)
        {
            editedNotes = new Dictionary<string,string>();
        }
        editedNotes[NoteName] = content;
        HttpContext.SetEditedNotes(editedNotes);
  
      
    }
}