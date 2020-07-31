using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using C19QCalcLib;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace C19QCalcLibTest
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
    [SuppressMessage("ReSharper", "RedundantCast")]
    public class SmokeTest
    {
        private readonly Instant _isolationNoSymptoms;
        private readonly Instant _isolationSymptoms;

        private readonly int isolationDaysNoSymptoms = 14;
        private readonly int isolationDaysSymptoms = 7;

        private readonly string cultureTagDefault = "en-GB";
        private readonly string timeZoneAcronymDefault = "GMT";
        private readonly string colorDone = "green";
        private readonly string colorNotDone = "orange";
        private readonly string NowDateTime = "04-05-2020 2:43 PM";

        private readonly IClock _clock;
        private readonly DateTimeZone _zoneGmt;

        public SmokeTest()
        {
            var tim = new LocalDateTime(2020, 5, 4, 14, 43, 0);  //NowDateTime, 04-05-2020 2:43 PM
            
            _zoneGmt = DateTimeZoneProviders.Tzdb["Europe/London"];
            _clock = new FakeClock(tim.InZoneStrictly(_zoneGmt).ToInstant());

            _isolationNoSymptoms = tim.InZoneStrictly(_zoneGmt).ToInstant().Plus(Duration.FromDays(isolationDaysNoSymptoms));
            _isolationSymptoms = tim.InZoneStrictly(_zoneGmt).ToInstant().Plus(Duration.FromDays(isolationDaysSymptoms));
        }

        private IndexFormProc GetForm(string cultureTag, string timeZoneAcronym, bool withoutDaylightSavings, string stillHasSymptoms, string startIsolation, string startSymptoms, out Dictionary<string, string>errors)
        {
            errors = null;
            var supportedTimeZones = new AppSupportedTimeZones();
            var form = new IndexFormProc(_clock, supportedTimeZones.GetTzDbName(timeZoneAcronym), cultureTag, withoutDaylightSavings);
            var paramList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(MxFormProc.ProgramErrorKey, ""),
                new KeyValuePair<string, object>(IndexFormProc.HasSymptomsKey, stillHasSymptoms),
                new KeyValuePair<string, object>(IndexFormProc.StartIsolationKey,  startIsolation),
                new KeyValuePair<string, object>(IndexFormProc.StartSymptomsKey, startSymptoms ),
            };
            errors = form.Validate(paramList.ToArray());
            return form;
        }

        [Fact]
        public void CtorTest()
        {
            Assert.Equal(NowDateTime, _clock.GetCurrentInstant().ToString(cultureTagDefault, _zoneGmt));
        }

        [Fact]
        public void Ref1NoSymptoms14DaysTest() //Ref# 1
        {
            Instant startIsolation = new LocalDateTime(2020, 05, 04, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var stillHasSymptoms = "no";

            var resultDaysRemaining = 14;
            var resultComment = $"The time remaining for your self-isolation is {resultDaysRemaining} days";
            var resultColor = colorNotDone;
            var resultSlider = 1.0;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), null, out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref2NoSymptoms1DayTest() //Ref# 2
        {
            Instant startIsolation = new LocalDateTime(2020, 04, 21, 14, 43, 00 ).InZoneStrictly(_zoneGmt).ToInstant();
            var stillHasSymptoms = "no";

            var resultDaysRemaining = 1;
            var resultComment = $"The time remaining for your self-isolation is {resultDaysRemaining} day";
            var resultColor = colorNotDone;
            var resultSlider = 0.07;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), null, out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double) calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref3NoSymptomsLessHourTest() //Ref# 3
        {
            Instant startIsolation = new LocalDateTime(2020, 04, 20, 14, 50, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var stillHasSymptoms = "no";

            var resultDaysRemaining = 0;
            var resultComment = "The time remaining for your self-isolation is less than an hour";
            var resultColor = colorNotDone;
            var resultSlider = 0.0;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), null, out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref4NoSymptomsDoneTest()  //Ref# 4
        {
            Instant startIsolation = new LocalDateTime(2020, 04, 20, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var stillHasSymptoms = "no";

            var resultDaysRemaining = 0;
            var resultComment = "Your self-isolation is now COMPLETE unless you have been advised otherwise";
            var resultColor = colorDone;
            var resultSlider = 0.0;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), null, out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref5SymptomsAfter7DaysTest()  //Ref# 5
        {
            var startIsolation = new LocalDateTime(2020, 04, 20, 14, 50, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var stillHasSymptoms = "yes";

            var resultDaysRemaining = 10;
            var resultComment = $"The time remaining for your self-isolation is {resultDaysRemaining} days";
            var resultColor = colorNotDone;
            var resultSlider = 0.71;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), null, out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref6SymptomsNowTest()       //Ref# 6
        {
            var startIsolation = new LocalDateTime(2020, 05, 4, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var stillHasSymptoms = "yes";

            var resultDaysRemaining = 10;
            var resultComment = $"The time remaining for your self-isolation is {resultDaysRemaining} days";
            var resultColor = colorNotDone;
            var resultSlider = 0.71;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), null, out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref7SymptomsNowAfter6DaysTest() //Ref# 7
        {
            var startIsolation = new LocalDateTime(2020, 04, 28, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = _clock.GetCurrentInstant();
            var stillHasSymptoms = "yes";

            string resultSymptoms = _clock.GetCurrentInstant().ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 10;
            var resultComment = $"The time remaining for your self-isolation is {resultDaysRemaining} days";
            var resultColor = colorNotDone;
            var resultSlider = 0.71;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref8SymptomsAtStartNoneAfter6DaysTest() //Ref# 8
        {
            var startIsolation = new LocalDateTime(2020, 04, 25, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = new LocalDateTime(2020, 04, 25, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();

            var stillHasSymptoms = "no";

            string resultSymptoms = startSymptoms.ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 1;
            var resultComment = $"The time remaining for your self-isolation is {resultDaysRemaining} day";
            var resultColor = colorNotDone;
            var resultSlider = 0.07;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref9SymptomsAtStartStillAfter6DaysTest()    //Ref# 9
        {
            var startIsolation = new LocalDateTime(2020, 04, 24, 14, 53, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = new LocalDateTime(2020, 04, 24, 14, 53, 00).InZoneStrictly(_zoneGmt).ToInstant();

            var stillHasSymptoms = "yes";

            string resultSymptoms = startSymptoms.ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 1;
            var resultComment = $"The time remaining for your self-isolation is {resultDaysRemaining} day";
            var resultColor = colorNotDone;
            var resultSlider = 0.07;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref10SymptomsAtStartDoneAfter7DaysTest()    //Ref# 10
        {
            var startIsolation = new LocalDateTime(2020, 04, 24, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = new LocalDateTime(2020, 04, 24, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();

            var stillHasSymptoms = "no";

            string resultSymptoms = startSymptoms.ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 0;
            var resultComment = $"Your self-isolation is now COMPLETE unless you have been advised otherwise";
            var resultColor = colorDone;
            var resultSlider = 0.0;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref11SymptomsAtStart26042020Test()    //Ref# 11
        {
            var startIsolation = new LocalDateTime(2020, 04, 23, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = new LocalDateTime(2020, 04, 23, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();

            var stillHasSymptoms = "yes";

            string resultSymptoms = startSymptoms.ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 1;
            var resultComment = "The time remaining for your self-isolation is 1 day";
            var resultColor = colorNotDone;
            var resultSlider = 0.07;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref12SymptomsAtStart25042020Test()    //Ref# 12
        {
            var startIsolation = new LocalDateTime(2020, 04, 22, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = new LocalDateTime(2020, 04, 23, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();

            var stillHasSymptoms = "yes";

            string resultSymptoms = startSymptoms.ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 1;
            var resultComment = "The time remaining for your self-isolation is 1 day";
            var resultColor = colorNotDone;
            var resultSlider = 0.07;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref13SymptomsAtStart24042020Test()    //Ref# 13
        {
            var startIsolation = new LocalDateTime(2020, 04, 21, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = new LocalDateTime(2020, 04, 23, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();

            var stillHasSymptoms = "yes";

            string resultSymptoms = startSymptoms.ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 1;
            var resultComment = "The time remaining for your self-isolation is 1 day";
            var resultColor = colorNotDone;
            var resultSlider = 0.07;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref14SymptomsAtStart21042020Test()    //Ref# 14
        {
            var startIsolation = new LocalDateTime(2020, 04, 21, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();
            var startSymptoms = new LocalDateTime(2020, 04, 21, 14, 43, 00).InZoneStrictly(_zoneGmt).ToInstant();

            var stillHasSymptoms = "yes";

            string resultSymptoms = startSymptoms.ToString(cultureTagDefault, _zoneGmt);
            var resultDaysRemaining = 1;
            var resultComment = "The time remaining for your self-isolation is 1 day";
            var resultColor = colorNotDone;
            var resultSlider = 0.07;

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation.ToString(cultureTagDefault, _zoneGmt), startSymptoms.ToString(cultureTagDefault, _zoneGmt), out var errors);
            Assert.NotNull(errors);
            Assert.Empty(errors);

            var calc = new CalcUk(new IsolateRecord("Fred", form.StartIsolation, form.HasSymptoms, form.StartSymptoms));

            Assert.Equal(14, calc.GetIsolationPeriodMax());
            Assert.Equal(stillHasSymptoms.Equals("yes", StringComparison.OrdinalIgnoreCase), calc.IsSymptomatic());
            Assert.Equal(resultSymptoms, form.StartSymptoms?.ToString(form.CultureTag, DateTimeZoneProviders.Tzdb[form.TzDbName]));

            Assert.Equal(resultDaysRemaining, calc.GetIsolationDaysRemaining(_clock.GetCurrentInstant(), out var colourName, out var comment));
            Assert.Equal(resultComment, comment);
            Assert.Equal(resultSlider, ((double)resultDaysRemaining / (double)calc.GetIsolationPeriodMax()), 2);
            Assert.Equal(resultColor, colourName);
        }

        [Fact]
        public void Ref15InvalidYearStartIsolationFailTest()    //Ref# 15
        {
            var startIsolation = "04-05-20 2:43 PM";
            var stillHasSymptoms = "no";

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation, null, out var errors);
            Assert.NotNull(form);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("StartIsolation", errors.Keys.ToArray()[0]);
            Assert.Equal("Please try again with a valid date/time like 14-04-2020 10:45 AM", errors.Values.ToArray()[0]);
        }

        [Fact]
        public void Ref16InvalidPMStartIsolationFailTest()    //Ref# 16
        {
            var startIsolation = "04-05-2020 2:43 XM";
            var stillHasSymptoms = "no";

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation, null, out var errors);
            Assert.NotNull(form);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("StartIsolation", errors.Keys.ToArray()[0]);
            Assert.Equal("Please try again with a valid date/time like 14-04-2020 10:45 AM", errors.Values.ToArray()[0]);
        }

        [Fact]
        public void Ref17InvalidPMStartSymptomsFailTest()    //Ref# 17
        {
            var startIsolation = "04-05-2020 2:43 PM";
            var startSymptoms = "04-05-2020 2:43 XM";

            var stillHasSymptoms = "no";

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation, startSymptoms, out var errors);
            Assert.NotNull(form);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("StartSymptoms", errors.Keys.ToArray()[0]);
            Assert.Equal("Please try again with a valid date/time like 14-04-2020 10:45 AM", errors.Values.ToArray()[0]);
        }

        [Fact]
        public void Ref18StartIsolationAfterNowFailTest()    //Ref# 18
        {
            var startIsolation = "04-05-2020 2:50 PM";
            var stillHasSymptoms = "no";

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation, null, out var errors);
            Assert.NotNull(form);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("StartIsolation", errors.Keys.ToArray()[0]);
            Assert.Equal("This value is after the current time. Please try again with a value before 04-05-2020 2:43 PM", errors.Values.ToArray()[0]);
        }

        [Fact]
        public void Ref19StartSymptomsAfterNowFailTest()    //Ref# 19
        {
            var startIsolation = "04-05-2020 2:43 PM";
            var startSymptoms = "04-05-2020 2:50 PM";

            var stillHasSymptoms = "no";

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation, startSymptoms, out var errors);
            Assert.NotNull(form);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("StartSymptoms", errors.Keys.ToArray()[0]);
            Assert.Equal("This value is after the current time. Please try again with a value before 04-05-2020 2:43 PM", errors.Values.ToArray()[0]);
        }

        [Fact]
        public void Ref20StartIsolationAfterStartSymptomsFailTest()    //Ref# 20
        {
            var startIsolation = "04-05-2020 2:43 PM";
            var startSymptoms = "04-05-2020 2:41 PM";

            var stillHasSymptoms = "no";

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation, startSymptoms, out var errors);
            Assert.NotNull(form);
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("StartSymptoms", errors.Keys.ToArray()[0]);
            Assert.Equal("This value is before the start of your self-isolation. If your symptoms started before you entered self-isolation then enter 04-05-2020 2:43 PM", errors.Values.ToArray()[0]);
        }

        [Fact]
        public void Ref21MultiFailTest()    //Ref# 21
        {
            var startIsolation = "04-05-2020 2:50 PM";
            var startSymptoms = "04-05-2020 2:43 PM";

            var stillHasSymptoms = "gg";

            var form = GetForm(cultureTagDefault, timeZoneAcronymDefault, false, stillHasSymptoms, startIsolation, startSymptoms, out var errors);
            Assert.NotNull(form);
            Assert.NotNull(errors);
            Assert.Equal(3, errors.Count);
            Assert.Equal("HasSymptoms", errors.Keys.ToArray()[0]);
            Assert.Equal("Please enter either 'yes' or 'no'", errors.Values.ToArray()[0]);
            Assert.Equal("StartIsolation", errors.Keys.ToArray()[1]);
            Assert.Equal("This value is after the current time. Please try again with a value before 04-05-2020 2:43 PM", errors.Values.ToArray()[1]);
            Assert.Equal("StartSymptoms", errors.Keys.ToArray()[2]);
            Assert.Equal("You must set the start of your self-isolation before giving the start of your symptoms", errors.Values.ToArray()[2]);
        }
    }
}