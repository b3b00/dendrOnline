using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dendrOnline;

public class NoteHeader
{
    public string Id { get; set; }
    
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public long LastUpdatedTS { get; set; }
    
    public long CreatedTS { get; set; }

    public string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(NoteParser.HeaderDelimiter)
            .Append("id: ").AppendLine(Id)
            .Append("title: ").AppendLine(Title)
            .Append("desc: ").AppendLine(Description)
            .Append("updated: ").AppendLine(LastUpdatedTS.ToString())
            .Append("created: ").AppendLine(CreatedTS.ToString())
            .AppendLine(NoteParser.HeaderDelimiter);
        return builder.ToString();
    }
} 


public class Note
{
    public NoteHeader Header { get; set; }
    
    public string Body { get; set; }
}

public class NoteParser
{
    public const string HeaderDelimiter = "---"; 
    
    public Note Parse(string content)
    {
        var headContent = ExtractHeaderContent(content);
        var header = ParseHeader(headContent.lines);
        var bodyLines = content.GetAllLines().Skip(headContent.end);
        string body = string.Join("\n", bodyLines);
        return new Note()
        {
            Header = header,
            Body = body
        };
    }

    public NoteHeader ParseHeader(List<string> headerLines)
    {
        NoteHeader header = new NoteHeader();
        foreach (var headerLine in headerLines)
        {
            ParseLine<string>("id", headerLine, header, (header, value) => header.Id = value);
            ParseLine<string>("title", headerLine, header, (header, value) => header.Title = value);
            ParseLine<string>("desc", headerLine, header, (header, value) => header.Description = value);
            ParseLine<long>("updated", headerLine, header, (header, value) => header.LastUpdatedTS = value);
            ParseLine<long>("created", headerLine, header, (header, value) => header.CreatedTS = value);
        }
        return header;
    }



    private (T value, bool ok) ParseLine<T>(string name, string line, NoteHeader header, Action<NoteHeader, T> setter)
    {
        if (line.StartsWith(name + ":"))
        {
            var end = line.Replace(name + ":", "");
            try
            {
                T value = end.GetValueAs<T>();
                setter(header, value);
            }
            catch
            {
                ;
            }
        }

        return (default(T), false);

    }

    private (List<string> lines,int start, int end) ExtractHeaderContent(string content)
    {
        List<string> header = new List<string>();
        var lines = content.GetAllLines();
        int i = 0;
        bool started = false;
        bool ended = false;
        int start = 0;
        int end = 0;
        while (i < lines.Count && !ended)
        {
            var line = lines[i];
            if (line == HeaderDelimiter)
            {
                if (!started)
                {
                    start = i;
                    started = true;
                }
                else
                {
                    if (started)
                    {
                        end = i;
                        ended = true;
                    }
                }
            }
            else
            {
                if (started && !ended)
                {
                    header.Add(line);
                }
            }
            i++;
        }

        return (header,start,end);
    }
}


public static class StringExtensions
{
    public static List<string> GetAllLines(this string source)
    {
        var lines = new List<string>();
        using (var reader = new StringReader(source))
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                lines.Add(line);
                line = reader.ReadLine();
            }
        }
        return lines;
    }
    
    public static T GetValueAs<T>(this String value)
    {
        return (T)Convert.ChangeType(value, typeof(T));
    }
}