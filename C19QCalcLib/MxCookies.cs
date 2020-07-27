using System;
using Microsoft.AspNetCore.Http;

namespace C19QCalcLib
{
    //public void ConfigureServices(IServiceCollection services)
    //{
    //      ...
    //      services.AddHttpContextAccessor();
    //      ...
    //}
    //
    //public class HomeController : Controller
    //{
    //    private readonly MxCookies _cookies;
    //
    //    public HomeController(IHttpContextAccessor httpContext)
    //    {
    //        _cookies = new MxCookies(httpContext);
    //    }
    //}


    public class MxCookies
    {
        public const int ExpiryDays = 3;
        public const int ExpiryEndOfSession = -1;
        public const string ErrorValue = "[error]";

        public IHttpContextAccessor Accessor { get; private set; }

        public MxCookies(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public string GetValue(string name)
        {
            string rc = ErrorValue;

            if ((Accessor != null) && (string.IsNullOrEmpty(name) == false))
            {
                if (Accessor.HttpContext.Request.Cookies.TryGetValue(name, out var result))
                    rc = result;
            }
            return rc;
        }

        public bool SetValue(string name, string value, int expiryDays = ExpiryEndOfSession, bool isEssential=false)
        {
            var rc = false;
            if ((Accessor != null) && (string.IsNullOrEmpty(name) == false) && (string.IsNullOrEmpty(value) == false))
            {
                var existingValue = GetValue(name);
                if (value.Equals(existingValue, StringComparison.Ordinal))
                    rc = true;
                else
                {
                    DateTimeOffset? expiry = null;
                    if (expiryDays != ExpiryEndOfSession)
                        expiry = new DateTimeOffset(DateTime.Now.AddDays(ExpiryDays));
                    Accessor.HttpContext.Response.Cookies.Append(name, value, new CookieOptions {Expires = expiry, IsEssential = isEssential});
                    // if ((Accessor.HttpContext.Request.Cookies.TryGetValue(name, out var result) == false) || (result != value))
                    //     rc = false; //see https://stackoverflow.com/questions/63114092/get-cookie-fails-immediately-after-set-cookie-in-asp-net-core
                    // else
                    rc = true;
                }
            }
            return rc;
        }

        public bool IsExists(string name)
        {
            bool rc = false;

            if ((Accessor != null) && (string.IsNullOrEmpty(name) == false))
                rc = Accessor.HttpContext.Request.Cookies.ContainsKey(name);
            return rc;
        }

        public bool Delete(string name)
        {
            bool rc = false;

            if (IsExists(name))
            {
                Accessor.HttpContext.Response.Cookies.Delete(name);
                rc = true;
            }
            return rc;
        }
    }
}
