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
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            SupportedAppTimeZones = new AppSupportedTimeZones();

            WithoutDaylightSaving = !SupportedAppTimeZones.IsDaylightSavingAuto(_httpContextAccessor);
            SelectedTimeZone = SupportedAppTimeZones.GetTimeZoneAcronymFromCookie(_httpContextAccessor);

            SupportedAppCultures = new AppSupportedCultures();
            SelectedCultureTab = SupportedAppCultures.GetCultureTabFromCookie(_httpContextAccessor);
        }

        public IActionResult OnPost()
        {
            SupportedAppTimeZones = new AppSupportedTimeZones();
            SupportedAppTimeZones.SetTimeZoneCookie(_httpContextAccessor, SelectedTimeZone, !WithoutDaylightSaving);

            SupportedAppCultures = new AppSupportedCultures();
            SupportedAppCultures.SetCultureCookie(_httpContextAccessor, SelectedCultureTab, SelectedCultureTab);

            return new RedirectToPageResult("Index");
        }
    }
}