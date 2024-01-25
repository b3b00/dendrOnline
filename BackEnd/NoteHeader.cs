using System;
using System.Text;

namespace BackEnd
{
    public class NoteHeader : IEquatable<NoteHeader>
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string TrimmedDescription => Description?.TrimEnd('\'')?.TrimStart('\'');
        
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

        public bool Equals(NoteHeader? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Title == other.Title && Description == other.Description && LastUpdatedTS == other.LastUpdatedTS && CreatedTS == other.CreatedTS;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NoteHeader)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, LastUpdatedTS, CreatedTS);
        }
    }
}