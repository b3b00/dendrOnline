using System;
using System.Collections.Generic;
using System.Text.Json;
using NFluent;
using SharpFileSystem;
using SharpFileSystem.FileSystems;
using Xunit;

namespace Tests;

public class HierarchyTests
{

    public static StubNotesService GetNotesService()
    {
        string rootDir = "/repository/notes/";
        List<string> items = new List<string>()
        {
            "perso.topic1.item1.md",
            "perso.topic1.item2.md",
            "perso.topic2.item1.md",
            "perso.topic2.item2.md",
            "perso.topic2.item2.subitem1.md",
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