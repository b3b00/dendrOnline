using System.Linq;
using BackEnd;
using Xunit;

namespace Tests;

public class HierarchyTests
{
    //[Fact]
    public void TestNoteHierarchy()
    {
        var notesService = new StubNotesService(typeof(HierarchyTests).Assembly, "/data/repository");
        var notes = notesService.GetNotes().GetAwaiter().GetResult();
        ;

    }
}