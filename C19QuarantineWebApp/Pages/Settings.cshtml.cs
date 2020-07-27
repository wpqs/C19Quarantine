using C19QCalcLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C19QuarantineWebApp.Pages
{
    public class SettingsModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public string ProgramError { get; private set; }

        public AppSupportedCultures SupportedAppCultures { get; private set; }
        public AppSupportedTimeZones SupportedAppTimeZones { get; private set; }

        [BindProperty]
        public bool WithoutDaylightSaving { get; set; }

        [BindProperty]
        public string SelectedTimeZone { get; set; }

        [BindProperty]
        public string SelectedCultureTab { get; set; }

        public SettingsModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            SupportedAppTimeZones = new AppSupportedTimeZones();
            SupportedAppCultures = new AppSupportedCultures();
        }

        public void OnGet()
        {
            var cookies = new MxCookies(_httpContextAccessor);

            WithoutDaylightSaving = !SupportedAppTimeZones.IsDaylightSavingAuto(cookies.GetValue(MxSupportedTimeZones.CookieName));
            SelectedTimeZone = SupportedAppTimeZones.GetTimeZoneAcronym(cookies.GetValue(MxSupportedTimeZones.CookieName));
            SelectedCultureTab = SupportedAppCultures.GetCultureTab(cookies.GetValue(MxSupportedCultures.CookieName));
        }

        public IActionResult OnPost()
        {
            var cookies = new MxCookies(_httpContextAccessor);
            cookies.SetValue(MxSupportedTimeZones.CookieName, SupportedAppTimeZones.GetTimeZoneEncodedValue(SelectedTimeZone, !WithoutDaylightSaving));

            var  culture = (MxSupportedCultures.HasRegion(SelectedCultureTab)) ? SelectedCultureTab  : SupportedAppCultures.GetNearestMatch(SelectedCultureTab);
            cookies.SetValue(MxSupportedCultures.CookieName, SupportedAppCultures.GetCulturesEncodedValue(culture, SelectedCultureTab));

            return new RedirectToPageResult("Index");
        }
    }
}