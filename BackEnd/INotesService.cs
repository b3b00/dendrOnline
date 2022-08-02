namespace BackEnd;

public interface INotesService
{
    string GetContent(string noteName);

    void SetContent(string noteName, string noteContent);

    List<string> GetNotes();

    INoteHierarchy GetHierarchy(List<string> notes);

}