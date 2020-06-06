using System;
using C19QCalcLib;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace C19QuarantineWebApp.Pages.Components.SettingsSummary
{
    public class SettingsSummaryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string text;
            try
            {
                var withoutDaylightSaving = (Request.Cookies[AppTimeZones.CookieWithoutDaylightSaving] != null) && (Request.Cookies[AppTimeZones.CookieWithoutDaylightSaving] == AppTimeZones.CookieWithoutDaylightSavingValueYes);
                var timeZoneAcronym = Request.Cookies[AppTimeZones.CookieTzName] ?? AppTimeZones.DefaultAcronym;

                if (withoutDaylightSaving)
                    text = timeZoneAcronym;
                else
                {
                    DateTimeZone zone = DateTimeZoneProviders.Tzdb[AppTimeZones.GetTzDbName(timeZoneAcronym)];
                    var daylightSavingNow = zone.IsDaylightSavingsTime(SystemClock.Instance.GetCurrentInstant());
                    text = (daylightSavingNow) ? AppTimeZones.GetDaylightSavingAcronym(timeZoneAcronym) : timeZoneAcronym;
                }
                text += ", ";
                text += Request.Cookies[AppCultures.CookieName] ?? AppCultures.DefaultTab;
            }
            catch (Exception e)
            {
                // ReSharper disable once RedundantAssignment
                text = e.Message;
                text = "[error]";
            }
            return View("Default", text);
        }
    }
}
