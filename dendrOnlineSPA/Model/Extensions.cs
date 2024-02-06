using System.Text.Json;

namespace dendrOnlinSPA.model;

public static class Extensions
{
        public static string GetRepositoryName(this HttpContext httpContext)
        {
            return httpContext.Session.GetString("repositoryName");
        }
        
        public static long GetRepositoryId(this HttpContext httpContext)
        {
            bool parsed = long.TryParse(httpContext.Session.GetString("repositoryId"), out long id);
            if (parsed)
            {
                return id;
            }

            return -1;
        }

        public static void SetRepositoryId(this HttpContext httpContext, long repositoryId)
        {
            httpContext.Session.SetString("repositoryId",repositoryId.ToString());
        }
        
        public static string GetUserName(this HttpContext httpContext)
        {
            return httpContext.Session.GetString("userName");
        }
        
        public static string GetCurrentNote(this HttpContext httpContext)
        {
            return httpContext.Session.GetString("currentNote");
        }

        public static void SetCurrentNote(this HttpContext httpContext, string currentNote)
        {
            httpContext.Session.SetString("currentNote",currentNote);
        }
        
        public static long GetUserId(this HttpContext httpContext)
        {
            var val = httpContext.Session.GetString("userId");
            if (!string.IsNullOrEmpty(val))
            {
                bool parsed = long.TryParse(val, out long id);
                if (parsed)
                {
                    return id;
                }
                return -1;
            }
            return -1;
        }
        
        public static Dictionary<string,string> GetEditedNotes(this HttpContext httpContext)
        {
            var editedNotes = httpContext.Session.GetObject<Dictionary<string, string>>("editedNotes");
            return editedNotes ?? new Dictionary<string, string>();
        }
        
        public static void SetEditedNotes(this HttpContext httpContext, Dictionary<string,string> editedNotes)
        {
            httpContext.Session.SetObject<Dictionary<string, string>>("editedNotes",editedNotes);
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
}