using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QCalcLib
{
    public class AppSupportedTimeZones : MxSupportedTimeZones
    {
        public const string AcronymGmt = "GMT";
        public const string AcronymCet = "CET";


        public const bool DefaultDaylightSavingAuto = true;

        public const string DefaultTzDbName = "Europe/London";
        public const string DefaultAcronym = AcronymGmt;

        public AppSupportedTimeZones()
        {
            Selected = DefaultAcronym;
            Items = new List<SelectListItem>
            {
                new SelectListItem("Greenwich Mean Time", AcronymGmt, (Selected == AcronymGmt)), 
                new SelectListItem("Central European Time", AcronymCet, (Selected == AcronymCet)) 
            };
        }

        public override string GetDefaultTimeZoneAcronym()
        {
            return DefaultAcronym;
        }

        public override bool GetDefaultDaylightSavingAuto()
        {
            return DefaultDaylightSavingAuto;        //default to applying daylight saving during summer time
        }

        public override string GetDaylightSavingName(string zoneAcronym)
        {
            var rc = "British Summer Time";
            if (string.IsNullOrEmpty(zoneAcronym) == false)
            {
                if (zoneAcronym == AcronymCet)
                    rc = "Central Europe Summer Time";
            }
            return rc;
        }
        public override string GetDaylightSavingAcronym(string zoneAcronym)
        {
            var rc = "BST";
            if (string.IsNullOrEmpty(zoneAcronym) == false)
            {
                if (zoneAcronym == AcronymCet)
                    rc = "CEST";
            }
            return rc;
        }
        public override string GetTzDbName(string zoneAcronym)
        {
            var rc = DefaultTzDbName;
            if (string.IsNullOrEmpty(zoneAcronym) == false)
            {
                if (zoneAcronym == AcronymCet)
                    rc = "Europe/Paris";
            }
            return rc;
        }
    }
}
