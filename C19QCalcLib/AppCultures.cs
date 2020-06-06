using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public class AppCultures
    {
        public const string CookieName = "C19CultureSetting";
        public const int  CookieExpiryDays = 3;

        public const string EnGb = "en-GB";
        public const string EnUs = "en-US";
        public const string FrCh = "fr-CH";
        public const string DeCh = "de-CH";
        public const string ItCh = "it-CH";

        public const string DefaultTab = EnGb;
        public string Selected { get; set; }
        public List<SelectListItem> Items { get; set; }

        public AppCultures()
        {
            Items = new List<SelectListItem>
            {
                new SelectListItem("English - United Kingdom", EnGb, (Selected == EnGb)),
                new SelectListItem("English - United States", EnUs, (Selected == EnUs)),
                new SelectListItem("Français - Suisse", FrCh, (Selected == FrCh)),
                new SelectListItem("Italien - Suisse", ItCh, (Selected == ItCh)),
                new SelectListItem("Allemand - Suisse", DeCh, (Selected == DeCh))
            };
            Selected = DefaultTab;
        }
    }
}
