using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public class AppSupportedCultures : MxSupportedCultures
    {
        public const string DefaultTab = En;
        public const string DefaultUiTab = En;

        public const string En = "en";
        public const string EnGb = "en-GB";
        public const string EnUs = "en-US";
        public const string FrCh = "fr-CH";
        public const string ItCh = "it-CH";
        public const string DeCh = "de-CH";

        public override string GetDefaultCultureTab()
        {
            return DefaultTab;
        }

        public override string GetDefaultUiCultureTab()
        {
            return DefaultUiTab;
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
