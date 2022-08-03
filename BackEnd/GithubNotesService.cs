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

    public override string GetContent(string noteName)
    {
        throw new NotImplementedException();
    }

    public override void SetContent(string noteName, string noteContent)
    {
        throw new NotImplementedException();
    }

    public override List<string> GetNotes()
    {
        gitHubClient.Repository.Content.GetAllContents();
    }
}