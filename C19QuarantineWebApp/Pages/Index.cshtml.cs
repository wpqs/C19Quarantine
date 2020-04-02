using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using C19QCalcLib;

namespace C19QuarantineWebApp.Pages
{
    public class IndexModel : PageModel
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<IndexModel> _logger;

        public int DaysInQuarantine { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var household = new List<Person>();
            household.Add(new Person());

            var calc = new CalcUk();
            DaysInQuarantine = calc.GetDaysInQuarantine(household.ToArray());
        }
    }
}
