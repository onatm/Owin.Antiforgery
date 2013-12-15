using Owin.Http;

namespace Owin
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseHttpMiddleware(this IAppBuilder builder, HttpMiddlewareOptions options = null)
        {
            var hostOptions = options ?? new HttpMiddlewareOptions();

            return builder.Use(typeof(HttpMiddleware), hostOptions);
        }
    }
}
