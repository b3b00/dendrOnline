using System;
using System.Text;

namespace BackEnd
{
    public class Note : IEquatable<Note>
    {
        public NoteHeader Header { get; set; }

        public string Body { get; set; }

        public string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Header.ToString())
                .AppendLine(Body);
            return builder.ToString();
        }

        public bool Equals(Note? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Header.Equals(other.Header) && Body == other.Body;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Note)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Header, Body);
        }
    }
}