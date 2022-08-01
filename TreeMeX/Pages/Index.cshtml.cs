using BackEnd;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;

namespace TreeMeX.Pages;

[ValidateAntiForgeryToken]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string PostContent { get; set; }  = "*empty*";

    public List<string> Notes { get; set; } = new List<string>();

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        
    }

    public IActionResult OnGet()
    {
        var notesService = HttpContext.RequestServices.GetService<INotesService>();
        Notes = notesService.GetNotes();
        
        if (!Request.IsHtmx())
        {
            return Page();
        }
        
        
        StringValues note = default;
        if (Request.Query.TryGetValue("note", out note))
        {
            PostContent = notesService.GetContent(note.First());
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