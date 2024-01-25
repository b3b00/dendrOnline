using System;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd
{
    public class NoteParser
    {
        public const string HeaderDelimiter = "---";

        public static Note Parse(string content)
        {
            var headContent = ExtractHeaderContent(content);
            var header = ParseHeader(headContent.lines);
            var bodyLines = content.GetAllLines().Skip(headContent.end+1);
            string body = string.Join("\n", bodyLines);
            return new Note()
            {
                Header = header,
                Body = body
            };
        }

        public static NoteHeader ParseHeader(List<string> headerLines)
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



        private static (T value, bool ok) ParseLine<T>(string name, string line, NoteHeader header,
            Action<NoteHeader, T> setter)
        {
            if (line.StartsWith(name + ":"))
            {
                var end = line.Replace(name + ": ", "");
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

        private static (List<string> lines, int start, int end) ExtractHeaderContent(string content)
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

            return (header, start, end);
        }
    }
}