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
        private readonly AppSupportedCultures _supportedCultures;
        private readonly AppSupportedTimeZones _supportedTimeZones;
        public SettingsSummaryViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _supportedCultures = new AppSupportedCultures();
            _supportedTimeZones = new AppSupportedTimeZones();
            _httpContextAccessor = httpContextAccessor;
        }
        public IViewComponentResult Invoke()
        {
            string text;
            try
            {
                var cookies = new MxCookies(_httpContextAccessor);

                var value = cookies.GetValue(MxSupportedTimeZones.CookieName);
                var timeZoneAcronym = _supportedTimeZones.GetTimeZoneAcronym(value);
                var withoutDaylightSaving = (!_supportedTimeZones.IsDaylightSavingAuto(value));
                if (withoutDaylightSaving)
                    text = timeZoneAcronym;
                else
                {
                    DateTimeZone zone = DateTimeZoneProviders.Tzdb[_supportedTimeZones.GetTzDbName(timeZoneAcronym)];
                    var daylightSavingNow = zone.IsDaylightSavingsTime(SystemClock.Instance.GetCurrentInstant());
                    text = (daylightSavingNow) ? _supportedTimeZones.GetDaylightSavingAcronym(timeZoneAcronym) : timeZoneAcronym;
                }

                value = cookies.GetValue(MxSupportedCultures.CookieName);
                var culture = _supportedCultures.GetCultureTab(value);
                var uiCulture = _supportedCultures.GetUiCultureTab(value);
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
