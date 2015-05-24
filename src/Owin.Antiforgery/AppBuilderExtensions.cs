namespace Owin.Antiforgery
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseAntiforgeryMiddleware(this IAppBuilder builder, AntiforgeryOptions options = null)
        {
            var hostOptions = options ?? new AntiforgeryOptions();

            return builder.Use(typeof(AntiforgeryMiddleware), hostOptions);
        }
    }
}
