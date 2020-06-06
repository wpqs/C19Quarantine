using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using C19QCalcLib;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

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

        [BindProperty, Required, Display(Name = "above"), StringLength(3, MinimumLength = 2)]
        public string HasSymptoms { get; set; }

        private IClock _clock;

        private string SelectedCultureTab { get; set; }
        private string SelectedTimeZone { get; set; }
        private bool  WithoutDaylightSavings { get; set;  }
        private string SelectedTzDbName { get; set; }



        public IndexModel()
        {
            _clock = SystemClock.Instance;
        }

        public void OnGet()
        {
            InitialiseSettings();

            TextColor = "black";
            Result = $"To calculate the number of days that you must remain in self-isolation provide the above information and then click the calculate button.";
            try
            {
                StartIsolation = _clock.GetCurrentInstant().ToString(SelectedCultureTab, DateTimeZoneProviders.Tzdb[SelectedTzDbName], WithoutDaylightSavings); 
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

            InitialiseSettings();

            var form = ProcessForm(SelectedTzDbName ?? AppTimeZones.DefaultTzDbName, SelectedCultureTab ?? AppCultures.DefaultTab);   //get time and culture from dropdowns on form

            if (ModelState.IsValid && (form != null))
            {
                TextColor = "red";
                try
                {
                    var nowInstance = _clock.GetCurrentInstant();
                    var record = new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms);
                    var calc = new CalcUk(record);

                    IsolationDaysMax = calc.GetIsolationPeriodMax();

                    if (calc.IsSymptomatic() && (form.StartSymptoms == null))
                        StartSymptoms = nowInstance.ToString(SelectedCultureTab, DateTimeZoneProviders.Tzdb[SelectedTzDbName], WithoutDaylightSavings); 
                    else
                        StartSymptoms = StartSymptoms;

                    ShowRange = true;
                    var span = calc.GetIsolationRemaining(nowInstance);
                    if (span.IsError())
                        ModelState.TryAddModelError(nameof(ProgramError), $"{MxFormProc.ProgramErrorMsg} 101: An internal error has been detected. Please report this problem");
                    else if (span.TotalMinutes > 0)
                    {
                        TextColor = "orange";
                        Result = $"The time remaining for your self-isolation is {span.ToStringRemainingTime()}"; 
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

        public IndexFormProc ProcessForm(string tzDbName, string cultureTab)
        {
            IndexFormProc rc = null;

            if (ModelState.IsValid)
            {
                var form = new IndexFormProc(_clock, tzDbName, cultureTab, WithoutDaylightSavings);

                var paramList = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>(nameof(ProgramError), ProgramError),
                    new KeyValuePair<string, object>(nameof(HasSymptoms), HasSymptoms),
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

        private void InitialiseSettings()
        {
            SelectedCultureTab = AppCultures.DefaultTab;
            if (Request.Cookies[AppCultures.CookieName] != null)
                SelectedCultureTab = Request.Cookies[AppCultures.CookieName];

            SelectedTimeZone = AppTimeZones.DefaultAcronym;
            if (Request.Cookies[AppTimeZones.CookieTzName] != null)
                SelectedTimeZone = Request.Cookies[AppTimeZones.CookieTzName];

            WithoutDaylightSavings = false; //apply daylight savings during summer time - default
            if (Request.Cookies[AppTimeZones.CookieWithoutDaylightSaving] != null)
                WithoutDaylightSavings = (AppTimeZones.CookieWithoutDaylightSavingValueYes == Request.Cookies[AppTimeZones.CookieWithoutDaylightSaving]);

            SelectedTzDbName = AppTimeZones.GetTzDbName(SelectedTimeZone);
        }
    }
}
