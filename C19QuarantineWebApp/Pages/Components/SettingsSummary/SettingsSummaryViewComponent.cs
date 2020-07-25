using System;
using C19QCalcLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace C19QuarantineWebApp.Pages.Components.SettingsSummary
{
    public class SettingsSummaryViewComponent : ViewComponent
    {
        private readonly MxCookies _cookies;
        public SettingsSummaryViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _cookies = new MxCookies(httpContextAccessor);
        }
        public IViewComponentResult Invoke()
        {
            string text;
            try
            {
                var supportedCultures = new AppSupportedCultures();
                var supportedTimeZones = new AppSupportedTimeZones();

                var value = _cookies.GetValue(MxSupportedTimeZones.CookieName);
                var timeZoneAcronym = supportedTimeZones.GetTimeZoneAcronym(value);
                var withoutDaylightSaving = (!supportedTimeZones.IsDaylightSavingAuto(value));
                if (withoutDaylightSaving)
                    text = timeZoneAcronym;
                else
                {
                    DateTimeZone zone = DateTimeZoneProviders.Tzdb[supportedTimeZones.GetTzDbName(timeZoneAcronym)];
                    var daylightSavingNow = zone.IsDaylightSavingsTime(SystemClock.Instance.GetCurrentInstant());
                    text = (daylightSavingNow) ? supportedTimeZones.GetDaylightSavingAcronym(timeZoneAcronym) : timeZoneAcronym;
                }

                value = _cookies.GetValue(MxSupportedCultures.CookieName);
                var culture = supportedCultures.GetCultureTab(value);
                var uiCulture = supportedCultures.GetUiCultureTab(value);
                text += ",c=";
                text += culture;
                text += "|uic=";
                text += uiCulture;

                    // Thread.CurrentThread.CurrentCulture = supportedCultures.GetCultureInfo(culture);
                    // Thread.CurrentThread.CurrentUICulture = supportedCultures.GetCultureInfo(uiCulture);

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
