namespace Owin.Antiforgery
{
    public class AntiforgeryOptions
    {
        public string CookieName { get; set; }

        public string FormFieldName { get; set; }

        public string HeaderName { get; set; }

        public int FailureCode { get; set; }

        public int MaxAge { get; set; }

        public string[] SafeMethods { get; set; }

        public string[] SafeContentTypes { get; set; }

        public AntiforgeryOptions()
        {
            CookieName = CsrfConstants.CookieName;
            FormFieldName = CsrfConstants.FormFieldName;
            HeaderName = CsrfConstants.HeaderName;
            FailureCode = CsrfConstants.FailureCode;
            MaxAge = CsrfConstants.MaxAge;
            SafeMethods = CsrfConstants.SafeMethods;
            SafeContentTypes = CsrfConstants.SafeContentTypes;
        }
    }
}
