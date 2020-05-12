using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace C19QuarantineWebApp.Pages.Components.FooterControl
{
    public class FooterControlViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var ver = "v" + typeof(FooterControlViewComponent).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            return View("Default", ver);
        }
    }
}
