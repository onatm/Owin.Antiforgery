using Owin;

namespace Owin.Http.Samples
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHttpMiddleware();
            app.Run(context =>
            {
                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
