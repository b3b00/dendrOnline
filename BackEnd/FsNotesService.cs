namespace BackEnd;

public class FsNotesService : AsbtractNotesService
{
    
    public string RootDirectory { get; set; }
    
    public FsNotesService(string rootDirectory)
    {
        RootDirectory = rootDirectory;
    }

    public override void SetRepository(string name, long id)
    {
        RootDirectory = name;
    }

    public override void SetAccessToken(string token)
    {
        ;
    }

    public override async Task<string> GetContent(string noteName)
    {
        string content = "";
        var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
        if (File.Exists(path))
        {
            content = File.ReadAllText(path);
        }
        return content;
    }

    public override async Task CreateNote(string noteName)
    {
        var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
        if (!File.Exists(path))
        {
            File.WriteAllText(path,GetHeader(noteName));
        }
    }

    public override async Task SetContent(string noteName, string noteContent)
    {
        var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
        if (File.Exists(path))
        {
            File.WriteAllText(path,noteContent);
        }
    }
    //C:\Users\olduh\DendronNotes
    public override async Task<List<string>> GetNotes()
    {
        var notedir = new DirectoryInfo(Path.Combine(RootDirectory, "notes"));
        var files = notedir.GetFiles("*.md");
        var list = files.Select(x => x.Name.Replace(".md", "")).ToList();
        return list;
    }

    
}