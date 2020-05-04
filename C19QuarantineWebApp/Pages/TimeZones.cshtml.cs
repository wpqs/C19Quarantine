using C19QCalcLib;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace C19QuarantineWebApp.Pages
{
    public class TimeZonesModel : PageModel
    {
        public string Result { get; private set; }
        public void OnGet()
        {
            Result = TimeZones.GetReport("<p>", "</p>", "&nbsp;&nbsp;&nbsp;", "<hr/>");
        }
    }
}