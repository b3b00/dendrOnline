using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BackEnd;
using SharpFileSystem;
using SharpFileSystem.FileSystems;

namespace Tests;

public class StubNotesService : AsbtractNotesService
{
    
        public string RootDirectory { get; set; }
        
        public IFileSystem FileSystem { get; set; }


        public StubNotesService(IFileSystem fileSystem, string rootDirectory)
        {
            FileSystem = fileSystem;
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

        private string GetNotePath(string noteName)
        {
            var path = Path.Combine(RootDirectory,"notes",$"{noteName}.md");
            return path;
        }

        public override async Task<string> GetContent(string noteName)
        {
            var path = GetNotePath(noteName);
            var content = FileSystem.ReadAllText(path);
            return content;
        }

        public override async Task<string> CreateNote(string noteName)
        {
            var path = GetNotePath(noteName);
            if (FileSystem.Exists(path))
            {
                Note note = new Note()
                {
                    Body = "*empty*",
                    Header = new NoteHeader(noteName)
                };
                var content = note.ToString();
                FileSystem.WriteAllText(path,content);
                return content;
            }
            return "";
        }

        public override async Task SetContent(string noteName, string noteContent)
        {
            var path = GetNotePath(noteName);
            if (FileSystem.Exists(path))
            {
                FileSystem.WriteAllText(path, noteContent);
            }
        }

        public override async Task<List<string>> GetNotes()
        {
            var notedir = RootDirectory+"/notes/";
            var files = FileSystem.GetEntitiesRecursive(FileSystemPath.Parse(notedir)).ToList();
            var list = files.Select(x => x.EntityName.Replace(".md", "")).ToList();
            return list;
        }


}