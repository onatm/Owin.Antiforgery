namespace Owin.Http
{
    public class HttpMiddlewareOptions
    {
        public string HostName { get; set; }

        public HttpMiddlewareOptions()
        {
            HostName = "localhost";
        }
    }
}
