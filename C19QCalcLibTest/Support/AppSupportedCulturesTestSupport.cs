using C19QCalcLib;

namespace C19QCalcLibTest.Support
{
    public class AppSupportedCulturesTestSupport : MxSupportedCultures
    {
        public AppSupportedCulturesTestSupport()
        {
            var cultures = new AppSupportedCultures();
            Selected = cultures.Selected;
            Items = cultures.Items;
        }
        public string GetCulturesValueToSetPublic(string cultureTab, string uiCultureTab, bool isCookieValue=true)
        {
            return GetCulturesValueToSet(cultureTab, uiCultureTab, isCookieValue);
        }

        public string GetCultureTabFromValuePublic(string value, bool valueIsCookie = true)
        {
            return GetCultureTabFromValue(value, valueIsCookie);
        }

        public string GetUiCultureTabFromValuePublic(string value, bool valueIsCookie = true)
        {
            return GetUiCultureTabFromValue(value, valueIsCookie);
        }

        public override string GetDefaultCultureTab()
        {
            return AppSupportedCultures.DefaultTab;
        }

        public override string GetDefaultUiCultureTab()
        {
            return AppSupportedCultures.DefaultTab;
        }
    }
}
