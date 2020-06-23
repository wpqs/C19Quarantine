using System;
using Microsoft.AspNetCore.Http;

namespace C19QCalcLib
{
    public static class MxCookies
    {
        public const int ExpiryDays = 3;
        public const int ExpiryEndOfSession = -1;
        public const string ErrorValue = "[error]";

        public static string GetValue(IHttpContextAccessor accessor, string name)
        {
            string rc = ErrorValue;

            if ((accessor != null) && (string.IsNullOrEmpty(name) == false))
            {
                if (accessor.HttpContext.Request.Cookies.TryGetValue(name, out var result))
                    rc = result;
            }
            return rc;
        }

        public static bool SetValue(IHttpContextAccessor accessor, string name, string value, int expiryDays = ExpiryEndOfSession, bool isEssential=false)
        {
            var rc = false;
            if ((accessor != null) && (string.IsNullOrEmpty(name) == false) && (string.IsNullOrEmpty(value) == false))
            {
                DateTimeOffset? expiry = null;
                if (expiryDays != ExpiryEndOfSession)
                    expiry = new DateTimeOffset(DateTime.Now.AddDays(ExpiryDays));
                accessor.HttpContext.Response.Cookies.Append(name, value, new CookieOptions { Expires = expiry, IsEssential = isEssential});
                rc = true;
            }
            return rc;
        }

        public static bool IsExists(IHttpContextAccessor accessor, string name)
        {
            bool rc = false;

            if ((accessor != null) && (string.IsNullOrEmpty(name) == false))
                rc = accessor.HttpContext.Request.Cookies.ContainsKey(name);
            return rc;
        }

        public static bool Delete(IHttpContextAccessor accessor, string name)
        {
            bool rc = false;

            if (MxCookies.IsExists(accessor, name))
            {
                accessor.HttpContext.Response.Cookies.Delete(name);
                rc = true;
            }
            return rc;
        }
    }
}
