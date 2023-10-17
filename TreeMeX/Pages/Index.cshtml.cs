﻿using System;
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
using Octokit;
using TreeMeX.Pages;

namespace dendrOnline.Pages;

[ValidateAntiForgeryToken]
public class IndexModel : BaseModel
{
    private readonly ILogger<IndexModel> _logger;
    
    [BindProperty(SupportsGet = true)]
    public string NoteName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string NoteQuery { get; set; }
    
    public string PostContent { get; set; }  = "*empty*";

    public bool EditorVisible { get; set; } = true;

    public bool ContentVisible { get; set; } = true;

    public string EditorStyle { get; set; } = "display: inline; width:40%";
    
    public string ContentStyle { get; set; } = "margin: auto 0;width: 60%; display: inline";
    
    public List<string> Notes { get; set; } = new List<string>();
    
    public bool UpdateEditor { get; set; }
    
    public INoteHierarchy NoteHierarchy { get; set; }
    
    [BindProperty]
    [HiddenInput] 
    public string CurrentNote { get; set; }
    
    public bool IsNoteDirty { get; set; }

    public string CurrentNoteShortName => CurrentNote.Split(new[] { '.' }).Last();
    
    public string CurrentNoteDescription { get; set; }
    
    public User GitHubUser { get; set; }

    public string RepositoryId => HttpContext.Session.GetString("repositoryId");
    public string RepositoryName => HttpContext.Session.GetString("repositoryName");
    

    private INotesService NotesService => HttpContext.RequestServices.GetService<INotesService>();
    public bool UpdatePreview { get; set; }
    public bool UpdateHierarchy { get; set; }


    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        
    }


    private void SetClient()
    {
        if (HttpContext.HasRepository())
        {
            NotesService.SetRepository(HttpContext.GetRepositoryName(),HttpContext.GetRepositoryId());
            NotesService.SetUser(HttpContext.GetUserName(),HttpContext.GetUserId());
            NotesService.SetAccessToken(HttpContext.GetGithubAccessToken());
        }
    }

    private async Task UpdateNotes()
    {
        SetClient();
        Notes = await NotesService.GetNotes();
        NoteHierarchy = NotesService.GetHierarchy(Notes,NoteQuery,NoteName, HttpContext.GetEditedNotes()?.Keys?.ToList());
        NoteHierarchy.Deploy(CurrentNote);
    }

    public async Task<IActionResult> OnPostFilterTree()
    {
        SetClient();
        
        Notes = await NotesService.GetNotes();
        SetClient();
        NoteHierarchy = NotesService.GetHierarchy(Notes,NoteQuery,NoteName, HttpContext.GetEditedNotes()?.Keys?.ToList());
        NoteHierarchy.Deploy(CurrentNote);
        return Partial("Hierarchy", NoteHierarchy);
    }

    public async Task<IActionResult> OnGetUndo()
    {
        IsNoteDirty = false;
        UpdateEditor = true;
        UpdateHierarchy = true;
        SetClient();
        var notesService = HttpContext.RequestServices.GetService<INotesService>();
        CurrentNote = Request.Query["note"].First();
        var content = await notesService.GetContent(CurrentNote);
         
        var editedNotes = HttpContext.GetEditedNotes();
        if (editedNotes != null && editedNotes.ContainsKey(CurrentNote))
        {
            editedNotes.Remove(CurrentNote);
            HttpContext.SetEditedNotes(editedNotes);
        }
        
        
        this.PostContent = content;

        var note = NoteParser.Parse(content);
        
        
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;
        CurrentNoteDescription = note.Header.TrimmedDescription;
        UpdatePreview = true;
        
        await UpdateNotes();
        
        return Partial("_Preview", this);
    }

    public async Task<IActionResult> OnGetDisplay()
    {
        SetClient();
        string toggle = Request.Query["toggle"].First();
        string note = Request.Query["note"].First();
        ExtractVisibility();
        CurrentNote = note;
        if (toggle == "editor")
        {
            EditorVisible = !EditorVisible;
            UpdateEditor = true;
            UpdateHierarchy = true;
        }
        if (toggle == "content")
        {
            ContentVisible = !ContentVisible;
        }
        HttpContext.Session.SetBool("EditorVisible",EditorVisible);
        HttpContext.Session.SetBool("ContentVisible",ContentVisible);
        SetDisplay();
        PostContent = await NotesService.GetContent(CurrentNote);
        return Partial("_Preview", this);
    }

    private void ExtractVisibility()
    {
        bool? editorVisibility = HttpContext.Session.GetBool("EditorVisible");
        EditorVisible = editorVisibility.HasValue ? editorVisibility.Value : true;
        bool? contentVisibility = HttpContext.Session.GetBool("ContentVisible");
        ContentVisible = contentVisibility.HasValue ? contentVisibility.Value : true;
    }

    private void SetDisplay()
    {
        
        if (EditorVisible && ContentVisible)
        {
            EditorStyle = "display:inline;width : 40%";
            ContentStyle = "margin: auto 0;width: 60%; display: inline";
        }
        else if (EditorVisible && !ContentVisible) {
            EditorStyle = "display:inline;width : 100%";
            ContentStyle = "margin: auto 0;width: 60%; display: none";
        }
        else if (!EditorVisible && ContentVisible) {
            EditorStyle = "display:none;";
            ContentStyle = "margin: auto 0;width: 100%; display: inline";
        }
        else {
            EditorStyle = "display:none;";
            ContentStyle = "margin: auto 0;width: 60%; display: none";
        }
    }
    
    public async Task<IActionResult> OnPostSave(string postContent, string CurrentNote)
    {
        IsNoteDirty = false;
        SetClient();
        await NotesService.SetContent(CurrentNote,postContent);
        
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;
        this.PostContent = postContent;
        await UpdateNotes();
        UpdateEditor = false;
        var parsed = NoteParser.Parse(postContent);
        CurrentNoteDescription = parsed.Header.TrimmedDescription;
        UpdatePreview = false;
        var editedNotes = HttpContext.GetEditedNotes();
        if (editedNotes != null && editedNotes.ContainsKey(CurrentNote))
        {
            editedNotes.Remove(CurrentNote);
            HttpContext.SetEditedNotes(editedNotes);
        }
        Notes = await NotesService.GetNotes();
        SetClient();
        NoteHierarchy = NotesService.GetHierarchy(Notes,NoteQuery,CurrentNote, HttpContext.GetEditedNotes()?.Keys?.ToList());
        NoteHierarchy.Deploy(CurrentNote);
        return Page();
    }

   


    public async Task<IActionResult> OnGetNewNote()
    {
        SetClient();
        string parent = Request.Query["parent"].First();
        string newNote = Request.Query["new"].First();
        var newContent = await NotesService.CreateNote(newNote);
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        this.GitHubUser = user;
        CurrentNote = newNote;
        
        await UpdateNotes();
        
        CurrentNote = newNote;
        PostContent = newContent;
        IsNoteDirty = true;
        var editedNotes = HttpContext.GetEditedNotes();
        if (editedNotes == null)
        {
            editedNotes = new Dictionary<string, string>();
        }

        editedNotes[CurrentNote] = newContent;
        HttpContext.SetEditedNotes(editedNotes);
        return Page();
    }

    public async Task<IActionResult> OnGet()
    {
        IsNoteDirty = false;
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        this.GitHubUser = user;
        
        
        UpdateEditor = false;
        if (!Request.IsHtmx())
        {
            if (!string.IsNullOrEmpty(NoteName))
            {
                CurrentNote = NoteName;
                await UpdateNotes();
            }
            else
            {
                CurrentNote = "root";
            }
            PostContent = await NotesService.GetContent(CurrentNote);
            var parsed = NoteParser.Parse(PostContent);
            CurrentNoteDescription = parsed.Header.TrimmedDescription;
            ExtractVisibility();
            SetDisplay();            
            await UpdateNotes();
            return Page();
        }
        
        if (!string.IsNullOrEmpty(NoteName))
        {
            
            CurrentNote = NoteName;
            await UpdateNotes();

            var editedNotes = HttpContext.GetEditedNotes();
            if (editedNotes != null && editedNotes.TryGetValue(CurrentNote, out var content))
            {
                PostContent = content;
                IsNoteDirty = true;
            }
            else
            {
                IsNoteDirty = false;
                PostContent = await NotesService.GetContent(NoteName);
            }
            var parsed = NoteParser.Parse(PostContent);
            CurrentNoteDescription = parsed.Header.TrimmedDescription;
            if (string.IsNullOrEmpty(CurrentNoteDescription))
            {
                CurrentNoteDescription = CurrentNote;
            }

            UpdatePreview = true;
            UpdateEditor = true;
            UpdateHierarchy = true;
        }

        ExtractVisibility();
        SetDisplay();
        return Partial("_Preview", this);
    }

    public async Task<IActionResult> OnDelete()
    {
        SetClient();
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        this.GitHubUser = user;
        
        
        UpdateEditor = false;
        UpdateHierarchy = true;
        if (!Request.IsHtmx())
        {
        }
        else
        {
            if (Request.Query.TryGetValue("note", out var noteName))
            {
                var notesService = HttpContext.RequestServices.GetService<INotesService>();
                if (notesService != null)
                {
                    notesService.SetAccessToken(accessToken);
                    await notesService.DeleteNote(noteName);
                    await UpdateNotes();
                    
                    var editedNotes = HttpContext.GetEditedNotes();
                    if (editedNotes != null && editedNotes.ContainsKey(noteName))
                    {
                        editedNotes.Remove(noteName);
                        HttpContext.SetEditedNotes(editedNotes);
                    }
                    
                    Notes = await NotesService.GetNotes();
                    SetClient();
                    NoteHierarchy = NotesService.GetHierarchy(Notes,NoteQuery,NoteName, HttpContext.GetEditedNotes()?.Keys?.ToList());
                    NoteHierarchy.Deploy(CurrentNote);
                    return Partial("Hierarchy", NoteHierarchy);
                }
            }
        }

        return new NoContentResult();
    }

    public async Task<IActionResult> OnPost(string PostContent)
    {
        IsNoteDirty = true;
        SetClient();
        var notesService = HttpContext.RequestServices.GetService<INotesService>();
        if (!Request.IsHtmx())
        {
            return Page();
        }

         CurrentNote = Request.Path.Value.Substring(1);
        
        var content = PostContent; 
        this.PostContent = content;

        var editedNotes = HttpContext.GetEditedNotes();
        if (editedNotes == null)
        {
            editedNotes = new Dictionary<string,string>();
        }
        editedNotes[CurrentNote] = content;
        HttpContext.SetEditedNotes(editedNotes);

        
        
        var note = NoteParser.Parse(content);
        
        GitHubClient client = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        var accessToken = HttpContext.GetGithubAccessToken();
        client.Credentials = new Credentials(accessToken);
        var user = await client.User.Current();
        GitHubUser = user;
        CurrentNoteDescription = note.Header.TrimmedDescription;
        UpdatePreview = true;
        UpdateHierarchy = true;
        await UpdateNotes();
        
        return Partial("_Preview", this);
    }

   
}