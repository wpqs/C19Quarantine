using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using Microsoft.AspNetCore.Http;
using NodaTime;

namespace C19QCalcLib
{
    public abstract class MxLib
    {
        public abstract Assembly GetResourcesAsm();

        public AppSupportedCultures SupportedCultures { get; }
        public AppSupportedTimeZones SupportedTimeZones { get; }

        public IHttpContextAccessor HttpContextAccessor { get; private set; }

        private ResourceManager _rm;

        private MxLib()
        {
            _rm = null;
            SupportedCultures = new AppSupportedCultures();
            SupportedTimeZones = new AppSupportedTimeZones();
        }

        protected MxLib(IHttpContextAccessor httpContextAccessor) : this()
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public string GetCultureTab()
        {
            var cookies = new MxCookies(HttpContextAccessor);
            return SupportedCultures.GetCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));
        }

        public string GetUiCultureTab()
        {
            var cookies = new MxCookies(HttpContextAccessor);
            return SupportedCultures.GetUiCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));
        }

        public bool SetCurrentCulture(string cultureTab = null, string uiCultureTab = null)
        {
            var rc = false;
            try
            {
                if (SupportedCultures.IsSupported(cultureTab))
                    Thread.CurrentThread.CurrentCulture = SupportedCultures.GetCultureInfo(cultureTab);
                if (SupportedCultures.IsSupported(uiCultureTab))
                     Thread.CurrentThread.CurrentUICulture = SupportedCultures.GetCultureInfo(uiCultureTab);
                rc = true;
            }
            catch (Exception)
            {
                //ignore
            }
            return rc;
        }

        public string InstantToString(Instant timeInstant, string cultureTab = null, bool withoutDaylightSaving = false, MxCultureInfo.FormatType formatType = MxCultureInfo.FormatType.DateTime, bool longFormat = false)
        {
            var rc = "[error]";

            try
            {
                var cookies = new MxCookies(HttpContextAccessor);
                if (SupportedCultures.IsSupported(cultureTab) == false)
                    cultureTab = SupportedCultures.GetCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));

                rc = timeInstant.ToString(cultureTab, DateTimeZoneProviders.Tzdb[SupportedTimeZones.GetTzDbName(cultureTab)]);
            }
            catch (Exception)
            {
                //ignore
            }
            return rc;
        }

        public Instant ParseTimeDate(string timeDate, bool withoutDaylightSaving=false, MxCultureInfo.FormatType formatType=MxCultureInfo.FormatType.DateTime, bool longFormat=false, string cultureTab=null)
        {
            var rc = ExtNodatime.InstantError;

            var cookies = new MxCookies(HttpContextAccessor);
            if (SupportedCultures.IsSupported(cultureTab) == false)
                cultureTab = SupportedCultures.GetCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));

            if (timeDate.ParseDateTime(DateTimeZoneProviders.Tzdb[SupportedTimeZones.GetTzDbName(cultureTab)], cultureTab, withoutDaylightSaving, formatType, longFormat, out var result))
                rc = result;

            return rc;
        }

        public Instant ParseTime(string time, Instant givenDate, bool withoutDaylightSaving=false, bool longFormat=false, string cultureTab=null)
        {
            var rc = ExtNodatime.InstantError;

            var cookies = new MxCookies(HttpContextAccessor);
            if (SupportedCultures.IsSupported(cultureTab) == false)
                cultureTab = SupportedCultures.GetCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));

            if (time.ParseTime(DateTimeZoneProviders.Tzdb[SupportedTimeZones.GetTzDbName(cultureTab)], cultureTab, withoutDaylightSaving, givenDate, longFormat, out var result))
                rc = result;

            return rc;
        }

        public Instant ParseDate(string date, bool longFormat=false, string cultureTab=null)
        {
            var rc = ExtNodatime.InstantError;

            var cookies = new MxCookies(HttpContextAccessor);
            if (SupportedCultures.IsSupported(cultureTab) == false)
                cultureTab = SupportedCultures.GetCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));

            if (date.ParseDate(DateTimeZoneProviders.Tzdb[SupportedTimeZones.GetTzDbName(cultureTab)], cultureTab, longFormat, out var result))
                rc = result;

            return rc;
        }

        public string GetText(string resourceName, string uiCultureTab=null, params Object[] list)     
        {
            var rc = "[not found]";
            try
            {
                if (SupportedCultures != null) 
                {
                    if (_rm == null)
                    {
                        var baseName = GetResourcesAsm().GetName().Name + ".Properties.Resources";
                        _rm = new ResourceManager(baseName, GetResourcesAsm());
                    }

                    var cookies = new MxCookies(HttpContextAccessor);
                    if (SupportedCultures.IsSupported(uiCultureTab) == false)
                        uiCultureTab = SupportedCultures.GetUiCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));
                    var uiCultureInfo = SupportedCultures.GetCultureInfo(uiCultureTab);
                    if (uiCultureInfo != null)
                    {
                        var res = _rm?.GetString(resourceName ?? "NotFound", uiCultureInfo) ?? "[Not Found]";
                        rc = string.Format(uiCultureInfo, res, list);
                    }
                }
            }
            catch (Exception e)
            {
                // ReSharper disable once UnusedVariable
                var msg = e.Message;
                //ignore
            }
            return rc;
        }
    }
}
