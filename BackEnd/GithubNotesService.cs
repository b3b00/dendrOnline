using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace BackEnd
{
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


        

        public override async Task<Result<(string content, string sha)>> GetContent(string name)
        {
            if (gitHubClient != null)
            {
                try
                {
                    var contents =
                        await gitHubClient.Repository.Content.GetAllContents(RepositoryId, $"notes/{name}.md");
                    if (contents.Any())
                    {   
                        var content = contents[0];
                        return (content.Content, content.Sha);
                    }
                    else
                    {
                        return Result<(string, string)>.Error(ResultCode.NotFound, $"note {name} not found");
                    }
                }
                catch (Exception e)
                {
                   return Result<(string, string)>.Error(ResultCode.InternalError, $"internal error : {e.Message}");
                }
            }
            return Result<(string,string)>.Error(ResultCode.InternalError, "unable to github connection");
        }

        public override async Task<Result<Note>> SetContent(string noteName, Note note)
        {
            if (gitHubClient != null)
            {
                var content = await NoteExists(noteName);
                if (!content.IsOk)
                {
                    return Result<Note>.TransformError<(bool,RepositoryContent),Note>(content);
                }
                if (content.TheResult.exists)
                {
                    if (content.TheResult.content.Sha != note.Sha && !string.IsNullOrEmpty(note.Sha))
                    {
                        return Result<Note>.Error(ResultCode.Conflict, ConflictCode.Modified,
                            $"note {noteName} has been modified");
                    }
                    
                    var newNote = NoteParser.Parse(note.ToString());
                    newNote.Header.LastUpdatedTS = DateTime.Now.ToTimestamp();
                    var request = new UpdateFileRequest($"DendrOnline : update {noteName}", newNote.ToString(),
                        content.TheResult.content.Sha);
                    var repositoryChange = await gitHubClient.Repository.Content.UpdateFile(RepositoryId, content.TheResult.content.Path, request);
                    newNote.Sha = repositoryChange.Content.Sha;
                    return newNote;
                }
                else
                {
                    var request =
                        new CreateFileRequest($"DendrOnline : new note : {noteName}", note.ToString(), "main");
                        var fileCreated =await gitHubClient.Repository.Content.CreateFile(RepositoryId,
                        "notes/" + noteName + ".md",
                        request);
                        note.Sha = fileCreated.Content.Sha;
                        return note;
                }
            }

            return Result<Note>.Error(ResultCode.InternalError, "unable to github connection");
        }

        public override async Task<string> CreateNote(string noteName)
        {
            if (gitHubClient != null)
            {
                var content = await NoteExists(noteName);
                if (!content.IsOk)
                {
                    return Result<string>.TransformError<(bool,RepositoryContent),string>(content);
                }
                if (!content.TheResult.exists)
                {
                    Note note = new Note()
                    {
                        Body = "*empty*",
                        Header = new NoteHeader(noteName)
                    };
                    var request =
                        new CreateFileRequest($"DendrOnline : new note : {noteName}", note.ToString(), "main");
                        await gitHubClient.Repository.Content.CreateFile(RepositoryId,
                        "notes/" + noteName + ".md",
                        request);
                    return note.ToString();
                }
            }

            return "";
        }

        private async Task<Result<IList<RepositoryContent>>> GetNoteFiles()
        {
            var contents = await gitHubClient.Repository.Content.GetAllContents(RepositoryId, "notes");
            return contents.Where(x => x.Name.EndsWith(".md")).ToList();
        }

        private async Task<Result<(bool exists, RepositoryContent content)>> NoteExists(string note)
        {
            var contents = await GetNoteFiles();
            if (!contents.IsOk)
            {
                return Result<(bool exists, RepositoryContent content)>.TransformError<IList<RepositoryContent>,(bool exists, RepositoryContent content)>(contents);
            }
            var content = contents.TheResult.FirstOrDefault(x => x.Name == note + ".md");
            return (content != null, content);
        }

        public override async Task<Result<List<string>>> GetNotes()
        {
            if (gitHubClient != null)
            {
                try
                {
                    var contents = await GetNoteFiles();
                    if (!contents.IsOk)
                    {
                        return Result<Result<List<string>>>.TransformError<IList<RepositoryContent>,List<string>>(contents);
                    }
                    return contents.TheResult.Select(x => x.Name.Replace(".md", "")).ToList();
                }
                catch (Exception e)
                {
                    return new List<string>();
                }
            }

            return new List<string>();
        }

        public override async Task<Result<Note>> DeleteNote(string noteName)
        {
            if (gitHubClient != null)
            {
                var content = await NoteExists(noteName);
                if (content.TheResult.exists)
                {
                    return await DeleteFile($"DendrOnline : delete note {noteName}", RepositoryId, $"notes/{noteName}.md",
                        content.TheResult.content.Sha);
                }
                else
                {
                    return Result<Note>.Error(ResultCode.NotFound, $"note {noteName} not found");
                }
            }
            return Result<Note>.Error(ResultCode.InternalError, "unable to github connection");
        }
        
        public override async Task<string> GetRepositoryName()
        {
            var repo = await gitHubClient.Repository.Get(RepositoryId);
            return repo.Name;
        }
        
        #region tooling
        
        private async Task<Result<Note>> DeleteFile(string message, long repositoryId, string fileName, string sha)
        {
            var creds = gitHubClient.Credentials;
            var user = await gitHubClient.User.Current();
            HttpClient client = new HttpClient();

            var repo = await gitHubClient.Repository.Get(repositoryId);
            
            var uri = new Uri(gitHubClient.BaseAddress, $"repos/{user.Login}/{repo.Name}/contents/{fileName}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
            
            GithubDeleteMessage githubDeleteMessage = new GithubDeleteMessage()
            {
                Message = message,
                Sha = sha
            };
            var jsonDeleteMessage = JsonSerializer.Serialize(githubDeleteMessage);
            var content = new StringContent(jsonDeleteMessage,Encoding.UTF8);
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationType.Bearer.ToString(),creds.Password);
            request.Content = content;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Headers.Add("owner",user.Login);
            request.Headers.UserAgent.TryParseAdd("DendrOnline's agent"); 
            var result = await client.SendAsync(request);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new ArgumentException($"'error while deleting note {fileName} : {result.StatusCode} - {result.ReasonPhrase}");
            }
            else
            {
                return Result<Note>.Ok();
            }
        }    
        
        
        #endregion
    }
}