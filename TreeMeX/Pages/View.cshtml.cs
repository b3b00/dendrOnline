using System;
using System.Threading.Tasks;
using BackEnd;
using dendrOnline;
using GitHubOAuthMiddleWare;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Octokit;

namespace TreeMeX.Pages;

public class ViewerModel : BaseModel
{
    
    private readonly ILogger<ViewerModel> _logger;
    
    [BindProperty(SupportsGet = true)]
    public string NoteName { get; set; }
    
    public string PostContent { get; set; }  = "*empty*";
    
    private INotesService NotesService => HttpContext.RequestServices.GetService<INotesService>();
    public async Task<IActionResult> OnGet()
    {
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;
        
        SetRepository();

        if (!string.IsNullOrEmpty(NoteName))
        {
            var editedNotes = HttpContext.GetEditedNotes();

            var content = "";
            if (!editedNotes.TryGetValue(NoteName, out content))
            {
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
}