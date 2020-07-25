using C19QCalcLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C19QuarantineWebApp.Pages
{
    public class SettingsModel : PageModel
    {
        private readonly MxCookies _cookies;
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string ProgramError { get; private set; }

        public AppSupportedCultures SupportedAppCultures { get; set; }
        public AppSupportedTimeZones SupportedAppTimeZones { get; set; }

        [BindProperty]
        public bool WithoutDaylightSaving { get; set; }

        [BindProperty]
        public string SelectedTimeZone { get; set; }

        [BindProperty]
        public string SelectedCultureTab { get; set; }

        public SettingsModel(IHttpContextAccessor httpContextAccessor)
        {
            _cookies = new MxCookies(httpContextAccessor);
        }

        public void OnGet()
        {
            SupportedAppTimeZones = new AppSupportedTimeZones();

            WithoutDaylightSaving = !SupportedAppTimeZones.IsDaylightSavingAuto(_cookies.GetValue(MxSupportedTimeZones.CookieName));
            SelectedTimeZone = SupportedAppTimeZones.GetTimeZoneAcronym(_cookies.GetValue(MxSupportedTimeZones.CookieName));

            SupportedAppCultures = new AppSupportedCultures();
            SelectedCultureTab = SupportedAppCultures.GetCultureTab(_cookies.GetValue(MxSupportedCultures.CookieName));
        }

        public IActionResult OnPost()
        {
            SupportedAppTimeZones = new AppSupportedTimeZones();
            _cookies.SetValue(MxSupportedTimeZones.CookieName, SupportedAppTimeZones.GetTimeZoneEncodedValue(SelectedTimeZone, !WithoutDaylightSaving));

             SupportedAppCultures = new AppSupportedCultures();
            _cookies.SetValue(MxSupportedCultures.CookieName, SupportedAppCultures.GetCulturesEncodedValue(SelectedCultureTab, SelectedCultureTab));

            return new RedirectToPageResult("Index");
        }
    }
}