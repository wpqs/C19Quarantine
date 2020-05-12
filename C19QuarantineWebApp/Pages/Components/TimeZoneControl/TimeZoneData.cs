using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace C19QuarantineWebApp.Pages.Components.TimeZoneControl
{
    public class TimeZoneData
    {
        public string Selected { get; set; }
        public List<SelectListItem> Items { get; set; }
        public string GetDefault() { return (Items.Count > 0) ? Items[0].Value : "[error]"; }

        public TimeZoneData()
        {
            Items = new List<SelectListItem>
            {
                new SelectListItem("Greenwich Mean Time", "GMT", (Selected == "GMT")),
                new SelectListItem("Central European Time", "CET", (Selected == "CET")),
                new SelectListItem("Indian Standard Time", "IST", (Selected == "IST"))
            };
            Selected = GetDefault();
        }
    }
}
