using System.Text;

namespace BackEnd
{
    public class Note
    {
        public NoteHeader Header { get; set; }

        public string Body { get; set; }

        public string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(Header.ToString())
                .AppendLine(Body);
            return builder.ToString();
        }
    }
}