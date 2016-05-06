using Microsoft.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Owin.Antiforgery
{
    public class AntiforgeryMiddleware : OwinMiddleware
    {
        private readonly AntiforgeryOptions _options;

        public AntiforgeryMiddleware(OwinMiddleware next, AntiforgeryOptions options)
            : base(next)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            _options = options;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.Headers.Add("Vary", new[] { "Cookie" });

            var tokenCookie = context.Request.Cookies[_options.CookieName];

            string realToken;

            if (string.IsNullOrEmpty(tokenCookie))
            {
                realToken = RegenerateToken(context);
            }
            else
            {
                realToken = tokenCookie;
            }

            if (realToken.Length != CsrfConstants.TokenLength)
            {
                realToken = RegenerateToken(context);
            }

            if (_options.SafeMethods.Contains(context.Request.Method))
            {
                await Next.Invoke(context);
                return;
            }

            if (OptionsContainIgnoredUrls(context.Request))
            {
                await Next.Invoke(context);
                return;
            }

            if (OptionsContainReferrer(context.Request))
            {
                await Next.Invoke(context);
                return;
            }

            if (context.Request.IsSecure)
            {
                var referer = context.Request.Headers.Get("Referer");

                if (string.IsNullOrEmpty(referer))
                {
                    context.Response.StatusCode = _options.FailureCode;
                    await context.Response.WriteAsync("A secure request contained no Referer or its value was malformed.");

                    return;
                }
            }

            var sentToken = context.Request.Headers.Get(_options.HeaderName);

            if (string.IsNullOrEmpty(sentToken))
            {
                if (_options.SafeContentTypes.Contains(context.Request.ContentType))
                {
                    var form = await context.Request.ReadFormAsync();
                    var fieldList = form.GetValues(_options.FormFieldName);

                    if (fieldList != null)
                    {
                        sentToken = fieldList[0];
                    }
                    else
                    {
                        context.Response.StatusCode = _options.FailureCode;
                        await context.Response.WriteAsync("The CSRF token in the cookie doesn't match the one received in a form/header.");

                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = _options.FailureCode;
                    await context.Response.WriteAsync("Forbidden content type.");

                    return;
                }
            }

            if (sentToken.Length != realToken.Length || !realToken.Equals(sentToken))
            {
                context.Response.StatusCode = _options.FailureCode;
                await context.Response.WriteAsync("The CSRF token in the cookie doesn't match the one received in a form/header.");

                return;
            }

            await Next.Invoke(context);
        }

        private bool OptionsContainIgnoredUrls(IOwinRequest request)
        {
            return _options.CsrfIgnoredUrls.Any(ignoredUrl => request.Uri.ToString().ToLowerInvariant().Contains(ignoredUrl.ToLowerInvariant()));
        }

        private bool OptionsContainReferrer(IOwinRequest request)
        {
            if (!request.Headers.ContainsKey("Referer"))
            {
                return false;
            }

            var referer = request.Headers.Get("Referer");

            return _options.WhitelistedReferrerUrls.Any(whitelistedUrl => referer.ToLowerInvariant().Contains(whitelistedUrl.ToLowerInvariant()));
        }

        public string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public string RegenerateToken(IOwinContext context)
        {
            var token = GenerateToken();

            context.Response.Cookies.Append(_options.CookieName, token);

            return token;
        }
    }
}
