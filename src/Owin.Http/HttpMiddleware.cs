using Microsoft.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Owin.Http
{
    public class HttpMiddleware: OwinMiddleware
    {
        private readonly HttpMiddlewareOptions options;

        public HttpMiddleware(OwinMiddleware next, HttpMiddlewareOptions options)
            :base(next)
        {
            this.options = options;
        }

        public override async Task Invoke(IOwinContext context)
        {
            //var owinRequestMethod = Get<string>(environment, "owin.RequestMethod");
            //var owinRequestScheme = Get<string>(environment, "owin.RequestScheme");
            var owinRequestHeaders = context.Request.Get<IDictionary<string, string[]>>("owin.RequestHeaders");
            //var owinRequestPathBase = Get<string>(environment, "owin.RequestPathBase");
            //var owinRequestPath = Get<string>(environment, "owin.RequestPath");
            //var owinRequestQueryString = Get<string>(environment, "owin.RequestQueryString");
            //var owinRequestBody = Get<Stream>(environment, "owin.RequestBody");
            var owinRequestHost = GetHeader(owinRequestHeaders, "Host") ?? Dns.GetHostName();


            if (!owinRequestHost.Contains(options.HostName))
            {
                context.Response.StatusCode = 403;
                return;
            }

            await Next.Invoke(context);
        }

        private static string GetHeader(IDictionary<string, string[]> headers, string key)
        {
            string[] value;
            return headers.TryGetValue(key, out value) && value != null ? string.Join(",", value.ToArray()) : null;
        }
    }
}
