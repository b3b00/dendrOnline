using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace BackEnd
{

    public class GithubNotesService : AsbtractNotesService
    {
        private GitHubClient gitHubClient { get; set; }

        private long RepositoryId { get; set; }

        private string RepositoryName { get; set; }

        private new long UserId { get; set; }

        private new string UserName { get; set; }

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
                try
                {
                    var contents =
                        await gitHubClient.Repository.Content.GetAllContents(RepositoryId, $"notes/{noteName}.md");
                    if (contents.Any())
                    {
                        var content = contents.First();
                        return content.Content;
                    }
                }
                catch (Exception e)
                {
                    // TODO : better error reporting
                    return @"# root note not found !

This may not be a dendron repository";
                }
            }

            return "";
        }

        public override async Task SetContent(string noteName, string noteContent)
        {
            if (gitHubClient != null)
            {
                var content = await NoteExists(noteName);
                if (content.exists)
                {
                    var note = NoteParser.Parse(noteContent);
                    note.Header.LastUpdatedTS = DateTime.Now.ToTimestamp();


                    var request = new UpdateFileRequest($"DendrOnline : update {noteName}", noteContent,
                        content.content.Sha);
                    gitHubClient.Repository.Content.UpdateFile(RepositoryId, content.content.Path, request);
                }
                else
                {
                    var request =
                        new CreateFileRequest($"DendrOnline : new note : {noteName}", noteContent, "main");
                        await gitHubClient.Repository.Content.CreateFile(RepositoryId,
                        "notes/" + noteName + ".md",
                        request);
                }
            }
        }

        public override async Task<string> CreateNote(string noteName)
        {
            if (gitHubClient != null)
            {
                var contents = await gitHubClient.Repository.Content.GetAllContents(RepositoryId, $"notes/");
                var content = await NoteExists(noteName);
                if (!content.exists)
                {
                    Note note = new Note()
                    {
                        Body = "*empty*",
                        Header = new NoteHeader(noteName)
                    };
                    var request =
                        new CreateFileRequest($"DendrOnline : new note : {noteName}", note.ToString(), "main");
                    var task = await gitHubClient.Repository.Content.CreateFile(RepositoryId,
                        "notes/" + noteName + ".md",
                        request);
                    return note.ToString();
                }
            }

            return "";
        }

        private async Task<IList<RepositoryContent>> GetNoteFiles()
        {
            var contents = await gitHubClient.Repository.Content.GetAllContents(RepositoryId, "notes");
            return contents.Where(x => x.Name.EndsWith(".md")).ToList();
        }

        private async Task<(bool exists, RepositoryContent content)> NoteExists(string note)
        {
            var contents = await GetNoteFiles();
            var content = contents.FirstOrDefault(x => x.Name == note + ".md");
            return (content != null, content);
        }

        public override async Task<List<string>> GetNotes()
        {
            if (gitHubClient != null)
            {
                try
                {
                    var contents = await GetNoteFiles();
                    return contents.Select(x => x.Name.Replace(".md", "")).ToList();
                }
                catch (Exception e)
                {
                    return new List<string>();
                }
            }

            return new List<string>();
        }

        public override async Task DeleteNote(string noteName)
        {
            if (gitHubClient != null)
            {
                var content = await NoteExists(noteName);
                if (content.exists)
                {
                    var deleteRequest =
                        new DeleteFileRequest($"DendrOnline : delete note {noteName}", content.content.Sha);
                    await gitHubClient.Repository.Content.DeleteFile(RepositoryId,$"notes/{noteName}.md" , deleteRequest);
                }
            }
        }
    }
}