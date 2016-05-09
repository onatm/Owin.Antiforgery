namespace Owin.Antiforgery
{
    public static class CsrfConstants
    {
        // the name of CSRF cookie.
        public static readonly string CookieName = "csrf_token";
        // the name of the form field.
        public static readonly string FormFieldName = "csrf_token";
        // the name of CSRF header.
        public static readonly string HeaderName = "X-CSRF-Token";
        // the HTTP status code for the default failure handler.
        public static readonly int FailureCode = 400;
        // Max-Age in seconds for the default base cookie. 365 days.
        public static readonly int MaxAge = 365 * 24 * 60 * 60;

        public static readonly int TokenLength = 23;

        public static readonly string[] SafeMethods =
        {
            "GET",
            "HEAD",
            "OPTIONS",
            "TRACE"
        };

        public static readonly string[] SafeContentTypes = 
        {
            "application/x-www-form-urlencoded",
            "multipart/form-data"
        };
    }
}
