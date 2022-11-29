using Microsoft.AspNetCore.Http;

namespace dendrOnline;

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