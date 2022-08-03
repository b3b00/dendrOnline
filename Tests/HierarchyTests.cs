using System.Linq;
using BackEnd;
using Xunit;

namespace Tests;

public class HierarchyTests
{
    [Fact]
    public void TestNoteHierarchy()
    {
        var notesService = new FsNotesService(@"C:\Users\olduh\DendronNotes");
        var notes = notesService.GetNotes().GetAwaiter().GetResult();
        var hierarchy = notesService.GetHierarchy(notes);
    }
}