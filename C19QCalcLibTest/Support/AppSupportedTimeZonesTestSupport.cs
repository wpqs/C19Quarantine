using C19QCalcLib;

namespace C19QCalcLibTest.Support
{
    public class AppSupportedTimeZonesTestSupport : MxSupportedTimeZones
    {
        public AppSupportedTimeZonesTestSupport()
        {
            var timezones = new AppSupportedTimeZones();
            Selected = timezones.Selected;
            Items = timezones.Items;
        }

        public string GetTimeZoneValueToSetPublic(string timeZoneAcronym, bool daylightSavingAuto, bool isCookieValue = true)
        {
            return GetTimeZoneValueToSet(timeZoneAcronym, daylightSavingAuto, isCookieValue);
        }

        public string GetTimeZoneAcronymFromValuePublic(string value, bool valueIsCookie = true)
        {
            return GetTimeZoneAcronymFromValue(value, valueIsCookie);
        }

        public string GetDaylightSavingAutoFromValuePublic(string value, bool valueIsCookie = true)
        {
            return GetDaylightSavingAutoFromValue(value, valueIsCookie);
        }

        public override string GetDefaultTimeZoneAcronym()
        {
            return AppSupportedTimeZones.DefaultAcronym;
        }

        public override bool GetDefaultDaylightSavingAuto()
        {
            return AppSupportedTimeZones.DefaultDaylightSavingAuto;
        }
    }
}
