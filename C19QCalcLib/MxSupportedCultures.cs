using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C19QCalcLib
{
    public abstract class MxSupportedCultures
    {
        public abstract string GetDefaultCultureTab();
        public abstract string GetDefaultUiCultureTab();

        public const string CookieName = ".AspNetCore.Culture"; // / ASPNET_CULTURE;
        public const int CookieExpiryDays = 3;

        public const string CookieCultureKey = "c=";
        public const string CookieUiCultureKey = "uic=";
        public const string CookieKeySeparator = "|";
        public const string QueryCultureKey = "culture=";
        public const string QueryUiCultureKey = "ui-culture=";
        public const string QueryKeySeparator = "&";

        public string Selected { get; set; }
        public List<SelectListItem> Items { get; set; }

        public bool IsSupported(string cultureTab)
        {
            return (string.IsNullOrEmpty(cultureTab) == false) && (Items?.Find(a => a.Value == cultureTab) != null);
        }

        public string[] GetSupportedCultures()
        {
            var list = new List<string>();
            foreach (var culture in Items)
            {
                list.Add(culture.Value);    
            }
            return list.ToArray();
        }

        public CultureInfo[] GetSupportedCulturesInfo()
        {
            var list = new List<CultureInfo>();
            foreach (var culture in Items)
            {
                list.Add(MxCultureInfo.Instance.GetCultureInfo(culture.Value));
            }
            return list.ToArray();
        }

        public CultureInfo GetCultureInfo(string cultureTab)
        {
            CultureInfo rc = null;
            if ((IsSupported(cultureTab)))
                rc = MxCultureInfo.Instance.GetCultureInfo(cultureTab);
            return rc;
        }

        public bool SetCultureCookie(IHttpContextAccessor httpContextAccessor, string cultureTab, string uiCultureTab)
        {
            var rc = true;
            var updateValue = GetCulturesValueToSet(cultureTab, uiCultureTab);
            var existValue = MxCookies.GetValue(httpContextAccessor, CookieName);
            if (existValue != updateValue)
                rc = MxCookies.SetValue(httpContextAccessor, CookieName, updateValue, MxSupportedTimeZones.CookieExpiryDays, true);
            return rc;
        }

        public string GetCultureTabFromCookie(IHttpContextAccessor httpContextAccessor)
        {
            return GetCultureTabFromValue(MxCookies.GetValue(httpContextAccessor, CookieName));
        }

        public string GetUiCultureTabFromCookie(IHttpContextAccessor httpContextAccessor)
        {
            return GetUiCultureTabFromValue(MxCookies.GetValue(httpContextAccessor, CookieName));
        }

        public string GetCultureTabFromValue(string value, bool valueIsCookie = true)
        {
            var rc = GetDefaultCultureTab();

            var cultureKey = (valueIsCookie) ? CookieCultureKey : QueryCultureKey;
            var keySeparator = (valueIsCookie) ? CookieKeySeparator : QueryKeySeparator;

            if (string.IsNullOrEmpty(value) == false)
            {
                var start = value.IndexOf(cultureKey, StringComparison.Ordinal);
                var end = value.IndexOf(keySeparator, StringComparison.Ordinal);
                if ((start == 0) && (end != -1) && ((end - cultureKey.Length) > 0))
                {
                    var culture = value.Substring(cultureKey.Length, end - cultureKey.Length);
                    if (IsSupported(culture))
                        rc = culture;
                }
            }
            return rc;
        }

        public string GetUiCultureTabFromValue(string value, bool valueIsCookie = true)
        {
            var rc = GetDefaultUiCultureTab();

            if (string.IsNullOrEmpty(value) == false)
            {
                var uiCultureKey = (valueIsCookie) ? CookieUiCultureKey : QueryUiCultureKey;
                var keySeparator = (valueIsCookie) ? CookieKeySeparator : QueryKeySeparator;

                var key = keySeparator + uiCultureKey;
                var start = value.IndexOf(key, StringComparison.Ordinal);
                if ((start != -1) && ((start + key.Length) < (value.Length - 1)))
                {
                    var uiCulture = value.Substring(start + key.Length);
                    if (IsSupported(uiCulture))
                        rc = uiCulture;
                }
            }
            return rc;
        }

        public string GetCulturesValueToSet(string culture, string uiCulture, bool getCookieValue = true)
        {
            var cultureKey = (getCookieValue) ? CookieCultureKey : QueryCultureKey;
            var keySeparator = (getCookieValue) ? CookieKeySeparator : QueryKeySeparator;
            var uiCultureKey = (getCookieValue) ? CookieUiCultureKey : QueryUiCultureKey;

            var rc = $"{cultureKey}{GetDefaultCultureTab()}{keySeparator}{uiCultureKey}{GetDefaultCultureTab()}";

            if (IsSupported(culture) && IsSupported(uiCulture))
                rc = $"{cultureKey}{culture ?? GetDefaultCultureTab()}{keySeparator}{uiCultureKey}{uiCulture ?? GetDefaultCultureTab()}";

            return rc;
        }
        private static bool SetCultureCookie(PageModel page, string value)       //delete candidate
        {
            var rc = false;
            if ((page != null) && (string.IsNullOrEmpty(value) == false))
            {
                page.Response.Cookies.Append(MxSupportedCultures.CookieName, value, new CookieOptions { Expires = DateTime.Now.AddDays(MxSupportedCultures.CookieExpiryDays), IsEssential = true });
                rc = true;
            }
            return rc;
        }

        private static string GetCultureCookie(PageModel page)                   //delete candidate
        {
            string rc = null;
            if (page != null)
                rc = page.Request.Cookies[MxSupportedCultures.CookieName];
            return rc;
        }

        private static string GetCultureCookie(ViewComponent component)          //delete candidate
        {
            string rc = null;
            if (component != null)
                rc = component.Request.Cookies[MxSupportedCultures.CookieName];
            return rc;
        }
    }
}
