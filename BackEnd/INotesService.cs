using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd
{

    public interface INotesService
    {
        Task<string> GetContent(string noteName);


        Task CreateNote(string name);

        Task SetContent(string noteName, string noteContent);

        Task<List<string>> GetNotes();

        INoteHierarchy GetHierarchy(List<string> notes);

        void SetRepository(string name, long id);

        void SetAccessToken(string token);

    }
}