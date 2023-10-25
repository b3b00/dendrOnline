using System.Text.Json.Serialization;
using Octokit;

namespace dendrOnlineSPA.Model;

public class GHUser
{
    public GHUser(User user)
    {
        id = user.Id;
        Name = user.Name;
        Login = user.Login;
        Avatar = user.AvatarUrl;
    }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("login")]
    public string Login { get; set; }
    
    [JsonPropertyName("id")]
    public long id { get; set; }
    
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }
}