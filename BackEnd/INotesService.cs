using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd
{

    public interface INotesService
    {
        Task<string> GetContent(string noteName);


        // creates note and returns the default content
        Task<string> CreateNote(string name);

        Task SetContent(string noteName, string noteContent);

        Task<List<string>> GetNotes();

        Task<Note> GetNote(string noteName);
        
        Task DeleteNote(string noteName);

        INoteHierarchy GetHierarchy(List<string> notes, string filter, string currentNote, List<string> editedNotes);

        void SetRepository(string name, long id);

        void SetUser(string name, long id);
        
        void SetAccessToken(string token);

    }
}