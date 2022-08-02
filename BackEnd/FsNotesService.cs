namespace BackEnd;

public class FsNotesService : AsbtractNotesService
{
    
    public string RootDirectory { get; set; }
    
    public FsNotesService(string rootDirectory)
    {
        RootDirectory = rootDirectory;
    }
    
    public override string GetContent(string noteName)
    {
        string content = "";
        var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
        if (File.Exists(path))
        {
            content = File.ReadAllText(path);
        }
        return content;
    }

    public override void SetContent(string noteName, string noteContent)
    {
        File.WriteAllText(Path.Combine(RootDirectory,"notes",noteName+".md"),noteContent);
    }
    //C:\Users\olduh\DendronNotes
    public override List<string> GetNotes()
    {
        var notedir = new DirectoryInfo(Path.Combine(RootDirectory, "notes"));
        var files = notedir.GetFiles("*.md");
        var list = files.Select(x => x.Name.Replace(".md", "")).ToList();
        return list;
    }

    
}