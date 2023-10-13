using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace dendrOnline;

public static class Extensions
{

    public const string RepositoryName = "repositoryName";
    
    public const string RepositoryId = "repositoryId";
    
    public const string UserName = "userName";
    
    public const string UserId = "userId";

    public const string EditedNotes = "editedNotes";
    
        public static string GetRepositoryName(this HttpContext httpContext)
        {
            return httpContext.Session.GetString(RepositoryName);
        }
        
        public static long GetRepositoryId(this HttpContext httpContext)
        {
            bool parsed = long.TryParse(httpContext.Session.GetString(RepositoryId), out long id);
            if (parsed)
            {
                return id;
            }

            return -1;
        }
        
        public static string GetUserName(this HttpContext httpContext)
        {
            return httpContext.Session.GetString(UserName);
        }
        
        public static long GetUserId(this HttpContext httpContext)
        {
            return (long)httpContext.Session.GetInt32(UserId)!;
        }
        
        public static Dictionary<string,string> GetEditedNotes(this HttpContext httpContext)
        {
            return httpContext.Session.GetObject<Dictionary<string, string>>(EditedNotes);
        }
        
        public static void SetEditedNotes(this HttpContext httpContext, Dictionary<string,string> editedNotes)
        {
            httpContext.Session.SetObject<Dictionary<string, string>>(EditedNotes,editedNotes);
        }
        
        public static T? GetObject<T>(this ISession session, string key) {
            var rawData = session.GetString(key);
            if (!string.IsNullOrEmpty(rawData))
            {
                var data = JsonSerializer.Deserialize<T>(rawData);
                return data;
            }

            return default;
        }
        
        public static void SetObject<T>(this ISession session, string key, T value) {
            var rawData = JsonSerializer.Serialize(value);
            session.SetString(key,rawData);
        }

        public static bool HasRepository(this HttpContext httpContext)
        {
            return httpContext.GetRepositoryId() != -1;
        }
        
        

        public static bool? GetBool(this ISession session, string key)
        {
            if (session.TryGetValue(key, out byte[] value))
            {
                bool storedValue = value[0] == 1;
                
                return storedValue;
            }
            return null;
        }

        public static void SetBool(this ISession session, string key, bool value)
        {
            session.Set(key, new byte[]{(byte)(value ? 1 : 0)});
        }

        public static void Logout(this HttpContext context)
        {
            context.Session.Clear();
        }
}