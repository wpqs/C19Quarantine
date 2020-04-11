using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using C19QCalcLib;
using Microsoft.AspNetCore.Mvc;

namespace C19QuarantineWebApp.Pages
{
    public class IndexModel : PageModel
    {
        public string Result { get; set; }
        public string TextColour { get; set; }

        public string StartQuarantine { get; set; }
        public string StartSymptoms { get; set; }
        public string Temperature { get; set; }

        public void OnGet()
        {
            TextColour = "black";
            Result = $"To calculate the number of days that you must remain in self-isolation provide the above data and then click the calculate button.";
            StartQuarantine = DateTime.Now.ToString(UiValidation.DateTimeFormat);
            Temperature = $"0.0 {UiValidation.DegreesCelsiusSymbol}";
        }

        public void OnPost(string startQuarantine, string startSymptoms, string temperature)
        {
            var validation = new UiValidation();
            TextColour = "red";
            if ((Result = validation.GetFormErrors(startQuarantine, startSymptoms, temperature)) == null)
            { 
                var person = new Person("Fred", validation.SelfIsolationTime, validation.TemperatureValue, validation.SymptomsTime);
                var calc = new CalcUk(person);
                var days = calc.GetDaysInQuarantine(DateTime.Now);
                if (days < 0)
                    Result = $"Program error 100: An internal error has been detected. Please report this problem and try again";
                else if (days > 0)
                {
                    TextColour = "orange";
                    Result = $"You need to remain in self-isolation for another {days} DAYS unless you have been advised otherwise";
                }
                else
                {
                    TextColour = "green";
                    Result = $"Your self-isolation is now COMPLETE unless you have been advised otherwise";
                }
            }
        }
    }
}
