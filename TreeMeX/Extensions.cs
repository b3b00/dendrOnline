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
}