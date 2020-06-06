using System;
using C19QCalcLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C19QuarantineWebApp.Pages
{
    public class SettingsModel : PageModel
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string ProgramError { get; private set; }

        public AppCultures SupportedAppCultures { get; set; }
        public AppTimeZones SupportedAppTimeZones { get; set; }

        [BindProperty]
        public bool WithoutDaylightSaving { get; set; }

        [BindProperty]
        public string SelectedTimeZone { get; set; }

        [BindProperty]
        public string SelectedCulture { get; set; }
        public void OnGet()
        {
            WithoutDaylightSaving = false;
            if (Request.Cookies[AppTimeZones.CookieWithoutDaylightSaving] != null)
                WithoutDaylightSaving = (AppTimeZones.CookieWithoutDaylightSavingValueYes == Request.Cookies[AppTimeZones.CookieWithoutDaylightSaving]);

            SupportedAppTimeZones = new AppTimeZones();

            var selTimeZone = AppTimeZones.DefaultAcronym;
            if (Request.Cookies[AppTimeZones.CookieTzName] != null)
                selTimeZone = Request.Cookies[AppTimeZones.CookieTzName];
            SelectedTimeZone = selTimeZone;

            SupportedAppCultures = new AppCultures();

            var selCulture = AppCultures.DefaultTab;
            if (Request.Cookies[AppCultures.CookieName] != null)
                selCulture = Request.Cookies[AppCultures.CookieName];
            SelectedCulture = selCulture;
        }

        public IActionResult OnPost()
        {

            Response.Cookies.Append(AppTimeZones.CookieTzName, SelectedTimeZone ?? AppTimeZones.DefaultAcronym, new CookieOptions { Expires = DateTime.Now.AddDays(AppTimeZones.CookieExpiryDays), IsEssential = true });
            Response.Cookies.Append(AppTimeZones.CookieWithoutDaylightSaving, WithoutDaylightSaving ? AppTimeZones.CookieWithoutDaylightSavingValueYes : AppTimeZones.CookieWithoutDaylightSavingValueNo, new CookieOptions { Expires = DateTime.Now.AddDays(AppTimeZones.CookieExpiryDays), IsEssential = true });
            Response.Cookies.Append(AppCultures.CookieName, SelectedCulture ?? AppCultures.DefaultTab, new CookieOptions { Expires = DateTime.Now.AddDays(AppCultures.CookieExpiryDays), IsEssential = true });

            return new RedirectToPageResult("Index");
        }
    }
}