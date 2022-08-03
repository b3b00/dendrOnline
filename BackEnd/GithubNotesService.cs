using Octokit;

namespace BackEnd;

public class GithubNotesService : AsbtractNotesService
{
    private GitHubClient gitHubClient { get; set; }
    
    private long RepositoryId { get; set; }
    
    private string RepositoryName { get; set; } 
    public override void SetRepository(string name, long id)
    {
        RepositoryId = id;
        RepositoryName = name;
    }

    public override void SetAccessToken(string token)
    {
        gitHubClient = new GitHubClient(new ProductHeaderValue("dendrOnline"), new Uri("https://github.com/"));
        gitHubClient.Credentials = new Credentials(token);
    }

    public override async Task<string> GetContent(string noteName)
    {
        if (gitHubClient != null)
        {
            var contents = await gitHubClient.Repository.Content.GetAllContents(RepositoryId, $"notes/{noteName}.md");
            if (contents.Any())
            {
                var content = contents.First();
                return content.Content;
            }
        }

        return "";
    }

    public override async Task SetContent(string noteName, string noteContent)
    {
        if (gitHubClient != null)
        {
            var request = new CreateFileRequest($"new note : {noteName}", noteContent, "main");
            var task = await gitHubClient.Repository.Content.CreateFile(RepositoryId, "notes/"+noteName + ".md", request);
            ;
        }
    }

    public override async Task<List<string>> GetNotes()
    {
        if (gitHubClient != null)
        {
            try
            {
                var contents = await gitHubClient.Repository.Content.GetAllContents(RepositoryId, "notes");
                return contents.Where(x => x.Name.EndsWith(".md")).Select(x => x.Name.Replace(".md", "")).ToList();
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }
        return new List<string>();
    }
}