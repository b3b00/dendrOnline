
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd
{

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