using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public abstract class MxSupportedCultures
    {
        public abstract string GetCultureTabForNeutralCulture();
        public abstract string GetDefaultUiCultureTab();
        public abstract string GetNearestMatch(string cultureTab, bool forUiCulture = false);

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
            return (string.IsNullOrEmpty(cultureTab) == false) && (Items?.Find(a => a.Value.Equals(cultureTab, StringComparison.InvariantCultureIgnoreCase)) != null);
        }

        public static bool HasRegion(string cultureTab)
        {
            return ((cultureTab?.Length ?? 0) > 3) && (cultureTab?.Contains("-") ?? false);
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

        public string GetCultureTab(string encodedValue, bool valueIsCookie = true)
        {
            var rc = GetCultureTabForNeutralCulture();

            if (string.IsNullOrEmpty(encodedValue) == false)
            {
                var cultureKey = (valueIsCookie) ? CookieCultureKey : QueryCultureKey;
                var keySeparator = (valueIsCookie) ? CookieKeySeparator : QueryKeySeparator;

                var start = encodedValue.IndexOf(cultureKey, StringComparison.Ordinal);
                var end = encodedValue.IndexOf(keySeparator, StringComparison.Ordinal);
                if ((start == 0) && (end != -1) && ((end - cultureKey.Length) > 0))
                {
                    var culture = encodedValue.Substring(cultureKey.Length, end - cultureKey.Length);
                    if (IsSupported(culture))
                        rc = culture;
                }
            }
            return rc;
        }

        public string GetUiCultureTab(string encodedValue, bool valueIsCookie = true)
        {
            var rc = GetDefaultUiCultureTab();

            if (string.IsNullOrEmpty(encodedValue) == false)
            {
                var uiCultureKey = (valueIsCookie) ? CookieUiCultureKey : QueryUiCultureKey;
                var keySeparator = (valueIsCookie) ? CookieKeySeparator : QueryKeySeparator;

                var key = keySeparator + uiCultureKey;
                var start = encodedValue.IndexOf(key, StringComparison.Ordinal);
                if ((start != -1) && ((start + key.Length) < (encodedValue.Length - 1)))
                {
                    var culture = encodedValue.Substring(start + key.Length);
                    if (IsSupported(culture))
                        rc = culture;
                }
            }
            return rc;
        }

        public string GetCulturesEncodedValue(string culture, string uiCulture, bool getCookieValue = true)
        {
            var cultureKey = (getCookieValue) ? CookieCultureKey : QueryCultureKey;
            var keySeparator = (getCookieValue) ? CookieKeySeparator : QueryKeySeparator;
            var uiCultureKey = (getCookieValue) ? CookieUiCultureKey : QueryUiCultureKey;

            var rc = $"{cultureKey}{GetCultureTabForNeutralCulture()}{keySeparator}{uiCultureKey}{GetDefaultUiCultureTab()}";

            if (IsSupported(culture) && IsSupported(uiCulture))
                rc = $"{cultureKey}{culture ?? GetCultureTabForNeutralCulture()}{keySeparator}{uiCultureKey}{uiCulture ?? GetDefaultUiCultureTab()}";

            return rc;
        }
    }
}
