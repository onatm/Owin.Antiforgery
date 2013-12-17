namespace Owin.Http
{
    public class HttpMiddlewareOptions
    {
        public string CookieName { get; set; }
        public string FormFieldName { get; set; }
        public string HeaderName { get; set; }
        public int FailureCode { get; set; }
        public int MaxAge { get; set; }
        public string[] SafeMethods { get; set; }

        public HttpMiddlewareOptions()
        {
            CookieName = CsrfConstants.CookieName;
            FormFieldName = CsrfConstants.FormFieldName;
            HeaderName = CsrfConstants.HeaderName;
            FailureCode = CsrfConstants.FailureCode;
            MaxAge = CsrfConstants.MaxAge;
            SafeMethods = CsrfConstants.SafeMethods;
        }
    }
}
