using System;
using C19QCalcLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace C19QuarantineWebApp.Pages.Components.SettingsSummary
{
    public class SettingsSummaryViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SettingsSummaryViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IViewComponentResult Invoke()
        {
            string text;
            try
            {
                var supportedCultures = new AppSupportedCultures();
                var timezones = new AppSupportedTimeZones();

                var timeZoneAcronym = timezones.GetTimeZoneAcronymFromCookie(_httpContextAccessor);
                var withoutDaylightSaving = (!timezones.IsDaylightSavingAuto(_httpContextAccessor));

                if (withoutDaylightSaving)
                    text = timeZoneAcronym;
                else
                {
                    DateTimeZone zone = DateTimeZoneProviders.Tzdb[AppSupportedTimeZones.GetTzDbName(timeZoneAcronym)];
                    var daylightSavingNow = zone.IsDaylightSavingsTime(SystemClock.Instance.GetCurrentInstant());
                    text = (daylightSavingNow) ? AppSupportedTimeZones.GetDaylightSavingAcronym(timeZoneAcronym) : timeZoneAcronym;
                }
                var culture = supportedCultures.GetCultureTabFromCookie(_httpContextAccessor);
                var uiCulture = supportedCultures.GetUiCultureTabFromCookie(_httpContextAccessor);
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
