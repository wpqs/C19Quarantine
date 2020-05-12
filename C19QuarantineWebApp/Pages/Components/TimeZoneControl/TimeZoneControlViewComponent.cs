using Microsoft.AspNetCore.Mvc;

namespace C19QuarantineWebApp.Pages.Components.TimeZoneControl
{
    public class TimeZoneControlViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var zones = new TimeZoneData();
            if (Request.Cookies["C19TimeZoneSetting"] != null)
                zones.Selected = Request.Cookies["C19TimeZoneSetting"];
            return View("Default", zones);
        }
    }
}
