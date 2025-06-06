
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BackEnd
{

    public class FsNotesService : AsbtractNotesService
    {

        public override async Task<string> GetRepositoryName()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(RootDirectory);
            return dirInfo.Name;
        }

        public string RootDirectory { get; set; }

        public FsNotesService(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public override async Task<string> GetUserLogin() => "test";

        public override async Task AddImage(IFormFile file, string fileName)
        {
            
        }

        public override async Task<Result<IList<ImageAsset>>> GetImages(long repositoryId)
        {
            return  Result<IList<ImageAsset>>.Error(ResultCode.NotFound,ConflictCode.NoConflict,"no images");
        }

        public override void SetRepository(string name, long id)
        {
            RootDirectory = name;
        }

        public override void SetAccessToken(string token)
        {
        }

        public override async Task<Result<(string content, string sha)>> GetContent(string name)
        {
            string content = "";
            var path = Path.Combine(RootDirectory, "notes", name + ".md");
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
            }

            return (content,"nope");
        }

        public override async Task<string> CreateNote(string noteName)
        {
            var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
            if (!File.Exists(path))
            {
                Note note = new Note()
                {
                    Body = "*empty*",
                    Header = new NoteHeader(noteName)
                };
                File.WriteAllText(path, note.ToString());
                return note.ToString();
            }

            return "";
        }

        public override async Task<Result<Note>> SetContent(string noteName, Note note)
        {
            var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
            if (File.Exists(path))
            {
                File.WriteAllText(path, note.ToString());
            }

            return Result<Note>.Ok();
        }

        //C:\Users\olduh\DendronNotes
        public override async Task<Result<List<string>>> GetNotes()
        {
            var notedir = new DirectoryInfo(Path.Combine(RootDirectory, "notes"));
            var files = notedir.GetFiles("*.md");
            var list = files.Select(x => x.Name.Replace(".md", "")).ToList();
            return list;
        }

        public override async Task<Result<Note>> DeleteNote(string noteName)
        {
            var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Result<Note>.Ok();
        }
    }
}