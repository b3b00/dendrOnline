using System.Text.Json.Serialization;

namespace BackEnd
{
    public class GithubDeleteMessage
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
       
        [JsonPropertyName("sha")]
        public string Sha { get; set; }
    }
}