namespace dendrOnlineSPA;

public static class NoCors
{
    public static WebApplication UseNoCors(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await next.Invoke();
        });

        return app;
    }
}