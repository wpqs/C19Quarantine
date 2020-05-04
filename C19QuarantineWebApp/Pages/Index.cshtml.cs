using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using C19QCalcLib;
using Microsoft.AspNetCore.Mvc;

namespace C19QuarantineWebApp.Pages
{
    public class IndexModel : PageModel
    {
        public bool ShowRange { get; private set; }
        public int IsolationDaysMax { get; private set; }
        public int IsolationDaysRemaining { get; private set; }
        public string Result { get; private set; }
        public string TextColor { get; private set; }
        public string ProgramError { get; private set; }

        [BindProperty, Required, Display(Name = "start self-isolation")]
        public string StartIsolation { get; set; }

        [BindProperty, Display(Name = "start of symptoms")]
        public string StartSymptoms { get; set; }

        [BindProperty, Required, Display(Name = "body temperature"), StringLength(4, MinimumLength = 2)]
        public string Temperature { get; set; }

        public void OnGet()
        {
            TextColor = "black";
            Result = $"To calculate the number of days that you must remain in self-isolation provide the above data and then click the calculate button.";
            try
            {
                StartIsolation = DateTime.UtcNow.ConvertUtcToLocalTimeString(IndexForm.DateTimeFormat, "GMT Standard Time");
                ShowRange = false;
            }
            catch (Exception e)
            {
                ModelState.TryAddModelError(nameof(ProgramError), $"{MxFormProc.ProgramErrorMsg} 100: {e.Message}. Please report this problem");
            }
        }

        public IActionResult OnPost() 
        {
            IActionResult rc = Page();

            var form = ProcessForm("GMT Standard Time", "en-GB");   //get time and culture from dropdowns on form

            if (ModelState.IsValid && (form != null))
            {
                TextColor = "red";
                try
                {
                    var nowUtc = DateTime.UtcNow;
                    var record = new Record("Fred", form.StartIsolation, form.Temperature, form.StartSymptoms);
                    var calc = new CalcUk(record);

                    IsolationDaysMax = calc.GetIsolationPeriodMax();

                    if (calc.IsSymptomatic(form.Temperature) && (form.StartSymptoms == null))
                        StartSymptoms = nowUtc.ConvertUtcToLocalTime("GMT Standard Time").ToString(IndexForm.DateTimeFormat);
                    else
                        StartSymptoms = StartSymptoms;

                    ShowRange = true;
                    var span = calc.GetTimeSpanInIsolation(nowUtc);
                    if (span.IsError())
                        ModelState.TryAddModelError(nameof(ProgramError), $"{MxFormProc.ProgramErrorMsg} 101: An internal error has been detected. Please report this problem");
                    else if (span.TotalMinutes > 0)
                    {
                        TextColor = "orange";
                        Result = $"The time remaining for your self-isolation is {span.ToStringRemainingDaysHours()}";
                        IsolationDaysRemaining = ((int) span.TotalDays) + 1;
                    }
                    else
                    {
                        TextColor = "green";
                        Result = $"Your self-isolation is now COMPLETE unless you have been advised otherwise";
                        IsolationDaysRemaining = 0;
                    }
                }
                catch (Exception e)
                {
                    ModelState.TryAddModelError(nameof(ProgramError), $"{MxFormProc.ProgramErrorMsg} 102: {e.Message}. Please report this problem.");
                }
            }
            return rc;
        }

        public IndexForm ProcessForm(string timeZoneId, string culture)
        {
            IndexForm rc = null;

            if (ModelState.IsValid)
            {
                var form = new IndexForm(timeZoneId, culture);

                var paramList = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>(nameof(ProgramError), ProgramError),
                    new KeyValuePair<string, object>(nameof(Temperature), Temperature),
                    new KeyValuePair<string, object>(nameof(StartIsolation), StartIsolation),
                    new KeyValuePair<string, object>(nameof(StartSymptoms), StartSymptoms),
                };

                var errors = form.Validate(paramList.ToArray());
                if (errors == null)
                    ModelState.TryAddModelError(nameof(ProgramError), $"{MxFormProc.ProgramErrorMsg} 100: Invalid Property Names");
                else
                {
                    foreach (var error in errors)
                        ModelState.TryAddModelError(error.Key, error.Value);

                    if (form.IsValid())
                        rc = form;
                }
            }
            return rc;
        }
    }
}
