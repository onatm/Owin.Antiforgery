using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Owin.Http
{
    public class HttpMiddleware: OwinMiddleware
    {
        private readonly HttpMiddlewareOptions options;

        public HttpMiddleware(OwinMiddleware next, HttpMiddlewareOptions options)
            :base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            this.options = options;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.Headers.Add("Vary", new string[] { "Cookie" });

            string tokenCookie = context.Request.Cookies[options.CookieName];

            var realToken = "";

            if (string.IsNullOrEmpty(tokenCookie))
                realToken = ""; // Generate token
            else
                realToken = tokenCookie;

            // check realToken length

            if (context.Request.IsSecure)
            {
                string referer = context.Request.Headers.Get("Referer");

                if (string.IsNullOrEmpty(referer))
                {
                    context.Response.StatusCode = options.FailureCode;
                    await context.Response.WriteAsync("A secure request contained no Referer or its value was malformed");

                    return;
                }
            }

            if (options.SafeMethods.Contains(context.Request.Method))
                await Next.Invoke(context);

            await Next.Invoke(context);
        }
    }
}
