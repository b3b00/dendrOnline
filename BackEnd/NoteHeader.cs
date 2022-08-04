using System;
using System.Text;

namespace BackEnd
{
    public class NoteHeader
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public long LastUpdatedTS { get; set; }

        public long CreatedTS { get; set; }

        public NoteHeader()
        {
            
        }
        
        public NoteHeader(string name)
        {
            Title = name.Replace(".", "-");
            Description = "";
            var ts = DateTime.Now.ToTimestamp();
            LastUpdatedTS = ts;
            CreatedTS = ts;
            Id = GenerateId(10);
        }

        public string GenerateId(int len)
        {
            string choices = "abcdefghijklmnopqrstuvwxyz012345679ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder id = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < len; i++)
            {
                var x = rnd.Next(choices.Length);
                id.Append(choices[x]);
            }
            return id.ToString();
        }

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
}