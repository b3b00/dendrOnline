using System.Text.Json;
using BackEnd;

namespace dendrOnlineSPA.Controllers;

public static class FavoriteExtensions
{
    
    public static Favorite? GetFavorite(this HttpContext httpContext)
    {
        var fav = httpContext.Request.Cookies["dendron-favorite"];
        if (fav != null)
        {
            var favorite = JsonSerializer.Deserialize<Favorite>(fav);
            return favorite;
        }
        return null;
    }

    public static void SetFavorite(this HttpContext httpContext, long repositoryId, string name)
    {
        httpContext.Response.Cookies.Delete("dendron-favorite");
        var fav = new Favorite()
        {
            RepositoryName = name,
            Repository = repositoryId
        };
        var neverExpire = new DateTimeOffset(3030, 03, 30, 23, 00, 00,TimeSpan.Zero);
        httpContext.Response.Cookies.Append("dendron-favorite",JsonSerializer.Serialize(fav), new CookieOptions() { Expires = neverExpire, Secure = true});
    }
    
    public static void DeleteFavorite(this HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("dendron-favorite");
    }
    
}
