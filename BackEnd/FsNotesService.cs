namespace BackEnd;

public class FsNotesService : INotesService
{
    
    public string RootDirectory { get; set; }
    
    public FsNotesService(string rootDirectory)
    {
        RootDirectory = rootDirectory;
    }
    
    public string GetContent(string noteName)
    {
        var content = File.ReadAllText(Path.Combine(RootDirectory,"notes", noteName+".md"));
        return content;
    }

    public void SetContent(string noteName, string noteContent)
    {
        File.WriteAllText(Path.Combine(RootDirectory,noteName),noteContent);
    }
    //C:\Users\olduh\DendronNotes
    public List<string> GetNotes()
    {
        var notedir = new DirectoryInfo(Path.Combine(RootDirectory, "notes"));
        var files = notedir.GetFiles("*.md");
        return files.Select(x => x.Name.Replace(".md", "")).ToList();
    }
}