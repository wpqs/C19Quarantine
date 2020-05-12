using C19QCalcLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C19QuarantineWebApp.Pages
{
    public class SettingsModel : PageModel
    {
        public TimeZones Zones { get; set; }

        [BindProperty]
        public string Selected { get; set; }
        public void OnGet()
        {
            Zones = new TimeZones();

            var selected = Zones.GetDefault();
            if (Request.Cookies["C19TimeZoneSetting"] != null)
                selected = Request.Cookies["C19TimeZoneSetting"];
            Selected = selected;
        }

        public IActionResult OnPost()
        {
            var zones = new TimeZones();
            Response.Cookies.Append("C19TimeZoneSetting", Selected ?? zones.GetDefault(), new CookieOptions { IsEssential = true });
            return new RedirectToPageResult("Index");
        }
    }
}