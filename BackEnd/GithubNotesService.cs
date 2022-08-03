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
        throw new NotImplementedException();
    }

    public override async Task<List<string>> GetNotes()
    {
        if (gitHubClient != null)
        {
            var contents = await gitHubClient.Repository.Content.GetAllContents(RepositoryId,"notes");
            return contents.Where(x => x.Name.EndsWith(".md")).Select(x => x.Name.Replace(".md", "")).ToList();
        }
        return new List<string>();
    }
}