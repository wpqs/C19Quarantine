using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public class AppSupportedCultures : MxSupportedCultures
    {
        public const string DefaultTab = EnGb;
        public const string DefaultUiTab = En;

        public const string En = "en";
        public const string EnGb = "en-GB";
        public const string EnUs = "en-US";
        public const string FrCh = "fr-CH";
        public const string ItCh = "it-CH";
        public const string DeCh = "de-CH";

        public override string GetCultureTabForNeutralCulture()
        {
            return DefaultTab;
        }

        public override string GetDefaultUiCultureTab()
        {
            return DefaultUiTab;
        }

        public override string GetNearestMatch(string cultureTab, bool forUiCulture = false)
        {
            var rc = (forUiCulture) ? DefaultUiTab : DefaultTab;

            if (string.IsNullOrEmpty(cultureTab) == false)
            {
                var index = cultureTab.IndexOf('-');
                var neutralCultureTab = index == -1 ? cultureTab : cultureTab.Substring(0, index);

                if (neutralCultureTab.Equals(En, StringComparison.InvariantCultureIgnoreCase))
                    rc = (forUiCulture) ? En : EnGb;
                else if (neutralCultureTab.Equals("fr", StringComparison.InvariantCultureIgnoreCase))
                    rc = FrCh;
                else if (neutralCultureTab.Equals("de", StringComparison.InvariantCultureIgnoreCase))
                    rc = DeCh;
                else if (neutralCultureTab.Equals("it", StringComparison.InvariantCultureIgnoreCase))
                    rc = ItCh;
                else
                {
                    //use defaults set above
                }
            }

            return rc;
        }


        public AppSupportedCultures()
        {
            Selected = DefaultTab;
            Items = new List<SelectListItem>
            {       //list of all cultures supported by the App; description - tab
                new SelectListItem("English", En, (Selected == En)),                    //default 
                new SelectListItem("English - United Kingdom", EnGb, (Selected == EnGb)),
                new SelectListItem("English - United States", EnUs, (Selected == EnUs)),
                new SelectListItem("Français - Suisse", FrCh, (Selected == FrCh)),
                new SelectListItem("Italien - Suisse", ItCh, (Selected == ItCh)),
                new SelectListItem("Allemand - Suisse", DeCh, (Selected == DeCh))
            };
        }
    }
}
