using C19QuarantineWebApp.Pages.Components.TimeZoneControl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C19QuarantineWebApp.Pages
{
    public class SettingsModel : PageModel
    {
        public TimeZoneData Zones { get; set; }
        public string Selected { get; set; }
        public void OnGet()
        {
            Zones = new TimeZoneData();

            var selected = Zones.GetDefault();
            if (Request.Cookies["C19TimeZoneSetting"] != null)
                selected = Request.Cookies["C19TimeZoneSetting"];
            Selected = selected;
        }

        public IActionResult OnPost(string selected)
        {
            var zones = new TimeZoneData();

            if (string.IsNullOrEmpty(selected) == false)
                Selected = selected;
            Response.Cookies.Append("C19TimeZoneSetting", Selected ?? zones.GetDefault(), new CookieOptions { IsEssential = true });
            return new RedirectToPageResult("Index");
        }
    }
}