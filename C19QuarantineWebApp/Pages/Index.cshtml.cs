using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using C19QCalcLib;

namespace C19QuarantineWebApp.Pages
{
    public class IndexModel : PageModel
    {
        public string Result { get; private set; }
        public string TextColor { get; private set; }

        public string StartQuarantine { get; set; }
        public string StartSymptoms { get; set; }
        public string Temperature { get; set; }

        public void OnGet()
        {
            TextColor = "black";
            Result = $"To calculate the number of days that you must remain in self-isolation provide the above data and then click the calculate button.";
            try
            {
                var nowUtc =  DateTime.UtcNow;
                StartQuarantine = nowUtc.ConvertUtcToLocalTime(UiValidation.DateTimeFormat, "GMT Standard Time");
                Temperature = $"0.0";
            }
            catch (Exception e)
            {
                Result = $"Program error 100: An internal error has been detected. {e.Message}. Please report this problem and try again";
            }
        }

        public void OnPost(string startQuarantine, string startSymptoms, string temperature)
        {
            TextColor = "red";
            try
            {
                var validation = new UiValidation();
                if ((Result = validation.ProcessForm(startQuarantine, startSymptoms, temperature, "GMT Standard Time", "en-GB")) == null)
                {
                    var person = new Person("Fred", validation.SelfIsolationTime, validation.TemperatureValue, validation.SymptomsTime);
                    var calc = new CalcUk(person);
                    var span = calc.GetSpanInQuarantine(DateTime.UtcNow);
                    if (span.IsError())
                        Result = $"Program error 101: An internal error has been detected. Please report this problem and try again";
                    else if (span.TotalMinutes > 0)
                    {
                        TextColor = "orange";
                        Result = $"The time remaining for your self-isolation is {span.ToStringDaysHours()}";
                    }
                    else
                    {
                        TextColor = "green";
                        Result = $"Your self-isolation is now COMPLETE unless you have been advised otherwise";
                    }
                }
            }
            catch (Exception e)
            {
                Result = $"Program error 102: An internal error has been detected. {e.Message}. Please report this problem and try again";
            }
        }
    }
}
