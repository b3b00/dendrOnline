using System.Collections.Generic;
using NFluent;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using Xunit;

namespace Tests;

public class HierarchyTests
{
    [Fact]
    public void TestNoteHierarchy()
    {
        var notesService = GetNotesService();
        var notes = notesService.GetNotes().GetAwaiter().GetResult();
        Check.That(notes).CountIs(5);
        ;
        var hTopic1 = notesService.GetHierarchy(notes, "topic1");
        Check.That(hTopic1.Dump("  ")).IsEqualTo(@"  root
    perso
      perso.topic1
        perso.topic1.item2
        perso.topic1.item1


");
            
        var hTopic2 = notesService.GetHierarchy(notes, "topic2");
        Check.That(hTopic2.Dump("  ")).IsEqualTo(@"  root
    perso
      perso.topic2
        perso.topic2.item2
        perso.topic2.item1


");
        
   

    }

    private static StubNotesService GetNotesService()
    {
        string rootDir = "/repository/notes/";
        List<string> items = new List<string>()
        {
            "perso.topic1.item1.md",
            "perso.topic1.item2.md",
            "perso.topic2.item1.md",
            "perso.topic2.item2.md",
            "root.md"
        };

        MemoryFileSystem memFS = new MemoryFileSystem();
        memFS.CreateDirectoryRecursive(rootDir);

        foreach (var item in items)
        {
            memFS.WriteAllText(rootDir + item, item);
        }

        var notesService = new StubNotesService(memFS, "/repository");
        return notesService;
    }
}