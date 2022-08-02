using System.Diagnostics;

namespace BackEnd;

public abstract class AsbtractNotesService : INotesService
{
    public abstract string GetContent(string noteName);

    public abstract void SetContent(string noteName, string noteContent);

    public abstract List<string> GetNotes();
    
    public INoteHierarchy GetHierarchy(List<string> notes)
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
        return root;
    }
}