
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd
{

    public class eFsNotesService : AsbtractNotesService
    {

        public string RootDirectory { get; set; }

        public eFsNotesService(string rootDirectory)
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

        public override async Task SetContent(string noteName, string noteContent)
        {
            var path = Path.Combine(RootDirectory, "notes", noteName + ".md");
            if (File.Exists(path))
            {
                File.WriteAllText(path, noteContent);
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
}