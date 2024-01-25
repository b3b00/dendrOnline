using System.Linq;
using BackEnd;
using NFluent;
using SharpFileSystem.FileSystems;
using Xunit;

namespace Tests;

public class NoteParserTests
{
    [Fact]
    public void TestParse()
    {
        var fs = new EmbeddedResourceFileSystem(typeof(NoteParserTests).Assembly);
        var content = fs.ReadAllText("/data/header/extractheader.md");
        
        var note = NoteParser.Parse(content);
        Assert.NotNull(note?.Header);
        var header = note.Header;
        Assert.Equal("f3v7nwkqkzq8hh048ra1lgv",header.Id);
        Assert.Equal(1659597467981,header.LastUpdatedTS);
        Assert.Equal("Analyse",header.Title);
    }

    [Fact]
    public void TestParseBody()
    {
        var fs = new EmbeddedResourceFileSystem(typeof(NoteParserTests).Assembly);
        var content = fs.ReadAllText("/data/modes.md");
        
        var note = NoteParser.Parse(content);
        
        
        Assert.NotNull(note?.Header);
        Check.That(note.Body).Not.Contains("---");
        var lines = note.Body.GetAllLines();
        Check.That(lines.First()).Not.IsNullOrEmpty();
        var header = note.Header;
        Check.That(header.Id).IsEqualTo("f1nxlhz4ar5qq4megq44mgs");
        Check.That(header.LastUpdatedTS).IsEqualTo(1670244034114);
        Check.That(header.Title).IsEqualTo("modes & travaux");
    }

    [Fact]
    public void TestNoteSerialization()
    {
        Note note = new Note()
        {
            Body = "toto",
            Header = new NoteHeader()
            {
                CreatedTS = 1,
                Description = "description",
                Id = "id",
                LastUpdatedTS = 2,
                Title = "title"
            }
        };
        var serialization = note.ToString();
        
        var newNote = NoteParser.Parse(serialization);
        Check.That(newNote.Body).IsEqualTo(note.Body);
        Check.That(newNote.Header).IsEqualTo(note.Header);
    }
}