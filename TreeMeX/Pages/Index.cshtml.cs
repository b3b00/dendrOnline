using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TreeMeX.Pages;

[ValidateAntiForgeryToken]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string PostContent { get; set; }  = "**this is the initial content**";

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (Request.IsHtmx())
        {
            var content = Request.Query["PostContent"].First();
            this.PostContent = content;
            // var rnd = new Random();
            //
            // return Content($@"<div style=""color:red;margin auto 0"">{rnd.Next(100)}</div>");
            return Partial("_Preview",this);
        }

        return Page();
    }

    public IActionResult OnPost(string PostContent)
    {
        if (!Request.IsHtmx())
        {
            return Page();
        }

        var content = PostContent; // Request.Query["PostContent"].First();
        this.PostContent = content;
        return Partial("_Preview", this);
    }

   
}