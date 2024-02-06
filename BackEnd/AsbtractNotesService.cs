using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
{

    public abstract class AsbtractNotesService : INotesService
    {
        public long UserId { get; set; }
        
        public string UserName { get; set; }
        
        public abstract void SetRepository(string name, long id);
        public void SetUser(string name, long id)
        {
            UserId = id;
            UserName = name;
        }

        public abstract void SetAccessToken(string token);

        public abstract Task<Result<(string content, string sha)>> GetContent(string noteName);

        public async Task<Result<Note>> GetNote(string noteName)
        {
            var getContentResult = await GetContent(noteName);
            if (!getContentResult.IsOk)
            {
                return Result<Note>.TransformError<(string,string),Note>(getContentResult);
            }
            var note = NoteParser.Parse(getContentResult.TheResult.content);
            note.Header.Title = noteName;
            note.Sha = getContentResult.TheResult.sha;
            return note;
        }
        public abstract Task<string> CreateNote(string name);

        public abstract Task<Result<Note>> SetContent(string noteName, Note note);

        public abstract Task<Result<List<string>>> GetNotes();
        public abstract Task<Result<Note>> DeleteNote(string noteName);


        public string GetHeader(string noteName)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("---")
                .AppendLine($"id: todo")
                .AppendLine($"title: {noteName.Replace(".", "-")} ")
                .AppendLine("desc: ''")
                .AppendLine("updated: todo")
                .AppendLine("created: todo")
                .AppendLine("---");
            return b.ToString();
        }

        public async Task<Result<INoteHierarchy>> GetHierarchy(List<string> notes, string filter, string currentNote, List<string> editedNotes)
        {
            var ordered = notes.OrderBy(x => x.Length).ToList();
            ordered.Reverse();
            NodeNote root = new NodeNote("");
            foreach (var note in ordered)
            {
                if (note != "root")
                {
                    Debug.WriteLine($"add note {note}");

                    root.AddChild(note,currentNote,editedNotes);

                    Debug.WriteLine(root.Dump(""));
                }
            }

            
            root.Name = "root";
            Debug.WriteLine(root.Dump(""));
            if (!string.IsNullOrEmpty(filter) && root.Filter(filter) is NodeNote r)
            {
                return r;
            }
            

            return root;
        }
    }
}