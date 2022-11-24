using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd
{

    public abstract class AsbtractNotesService : INotesService
    {
        public abstract void SetRepository(string name, long id);

        public abstract void SetAccessToken(string token);

        public abstract Task<string> GetContent(string noteName);
        public abstract Task<string> CreateNote(string name);

        public abstract Task SetContent(string noteName, string noteContent);

        public abstract Task<List<string>> GetNotes();


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

        public INoteHierarchy GetHierarchy(List<string> notes, string filter)
        {
            var ordered = notes.OrderBy(x => x.Length).ToList();
            ordered.Reverse();
            NodeNote root = new NodeNote("");
            foreach (var note in ordered)
            {
                if (note != "root")
                {
                    Debug.WriteLine($"add note {note}");

                    root.AddChild(note);

                    Debug.WriteLine(root.Dump(""));
                }
            }

            
            root.Name = "root";
            Debug.WriteLine(root.Dump(""));
            if (!string.IsNullOrEmpty(filter))
            {
                root = root.Filter(filter) as NodeNote;
            }

            return root;
        }
    }
}