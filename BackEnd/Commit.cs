using System;
using System.Text.Json.Serialization;
using Octokit;

namespace BackEnd
{
    public class Commit
    {
        [JsonPropertyName("author")]
        public GithubUser CommitAuthor { get; set; }

        [JsonPropertyName("committer")]
        public GithubUser Committer { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("tree")]
        public Tree Tree { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("sha")]
        public string Sha { get; set; }



        public Commit(GitHubCommit ghCommit)
        {
            Url = ghCommit.Url;
            Message = ghCommit.Commit.Message;
            CommitAuthor = new GithubUser(ghCommit.Commit.Author);
            Committer = new GithubUser(ghCommit.Commit.Committer);
            Tree = new Tree(ghCommit.Commit.Tree);
            Sha = ghCommit.Sha;
        }
    }
    
    public class GithubUser
    {
    
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("email")]
        public string Email { get; set; }
    
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("name")]
        public string Name { get; set; }
    
        public GithubUser(Committer author)
        {
            Email = author.Email;
            Name = author.Name;
        }
    }

    public class Tree
    {
        public Tree(GitReference reference)
        {
            Sha = reference.Sha;
            Url = reference.Url;
        }
        
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

}