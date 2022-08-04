using BackEnd;
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
}