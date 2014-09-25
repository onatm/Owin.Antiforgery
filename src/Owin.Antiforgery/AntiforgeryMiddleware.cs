using Microsoft.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Owin.Antiforgery
{
    public class AntiforgeryMiddleware : OwinMiddleware
    {
        private readonly AntiforgeryOptions options;

        public AntiforgeryMiddleware(OwinMiddleware next, AntiforgeryOptions options)
            : base(next)
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

            string realToken = "";

            if (string.IsNullOrEmpty(tokenCookie))
                realToken = RegenerateToken(context);
            else
                realToken = tokenCookie;

            if (realToken.Length != CsrfConstants.TokenLength)
                realToken = RegenerateToken(context);

            if (options.SafeMethods.Contains(context.Request.Method))
            {
                await Next.Invoke(context);
                return;
            }

            if (context.Request.IsSecure)
            {
                string referer = context.Request.Headers.Get("Referer");

                if (string.IsNullOrEmpty(referer))
                {
                    context.Response.StatusCode = options.FailureCode;
                    await context.Response.WriteAsync("A secure request contained no Referer or its value was malformed.");

                    return;
                }
            }

            string sentToken = context.Request.Headers.Get(options.HeaderName);

            if (string.IsNullOrEmpty(sentToken))
            {
                if (options.SafeContentTypes.Contains(context.Request.ContentType))
                {
                    var form = await context.Request.ReadFormAsync();
                    var fieldList = form.GetValues(options.FormFieldName);

                    if (fieldList != null)
                    {
                        sentToken = fieldList[0];
                    }
                    else
                    {
                        context.Response.StatusCode = options.FailureCode;
                        await context.Response.WriteAsync("The CSRF token in the cookie doesn't match the one received in a form/header.");

                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = options.FailureCode;
                    await context.Response.WriteAsync("Forbidden content type.");

                    return;
                }
            }

            if (sentToken.Length != realToken.Length || !realToken.Equals(sentToken))
            {
                context.Response.StatusCode = options.FailureCode;
                await context.Response.WriteAsync("The CSRF token in the cookie doesn't match the one received in a form/header.");

                return;
            }

            await Next.Invoke(context);
        }

        public string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string RegenerateToken(IOwinContext context)
        {
            string token = GenerateToken();

            context.Response.Cookies.Append(options.CookieName, token);

            return token;
        }
    }
}
