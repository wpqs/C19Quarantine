using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using C19QCalcLib;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace C19QCalcLibTest
{
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    public class IndexFormProcTest
    {
        private const string StdLocalTime = "01-01-2020 5:35 PM";
        private const int StdLocalTimeYear = 2020;
        private const int StdLocalTimeMonth = 1;
        private const int StdLocalTimeDay = 1;
        private const int StdLocalTimeHour = 17;
        private const int StdLocalTimeMins = 35;
        private const int StdLocalTimeSec = 00;


        private readonly IClock _clock;
        private readonly DateTimeZone _zoneGmt;

        public IndexFormProcTest()
        {
            _zoneGmt = DateTimeZoneProviders.Tzdb["Europe/London"];
            var tim = new LocalDateTime(StdLocalTimeYear, StdLocalTimeMonth, StdLocalTimeDay, StdLocalTimeHour, StdLocalTimeMins, StdLocalTimeSec);
            _clock = new FakeClock(tim.InZoneStrictly(_zoneGmt).ToInstant());
        }

        [Fact]
        public void ValidateTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.False(form.IsValid());

            var paramList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(MxFormProc.ProgramErrorKey, ""),
                new KeyValuePair<string, object>(IndexFormProc.HasSymptomsKey, "yes"),
                new KeyValuePair<string, object>(IndexFormProc.StartIsolationKey, StdLocalTime),
                new KeyValuePair<string, object>(IndexFormProc.StartSymptomsKey, StdLocalTime),
            };

            var errors = form.Validate(paramList.ToArray());

            Assert.NotNull(errors);
            Assert.Empty(errors);
            Assert.True(form.IsValid());

            Assert.True(form.HasSymptoms);
            Assert.Equal(_clock.GetCurrentInstant(), form.StartIsolation);
            Assert.Equal(_clock.GetCurrentInstant(), form.StartSymptoms);
        }

        [Fact]
        public void ValidateMissingKeyFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.False(form.IsValid());

            var paramList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(MxFormProc.ProgramErrorKey, ""),
                new KeyValuePair<string, object>(IndexFormProc.StartSymptomsKey, "01-01-20 17:35"),
            };

            var errors = form.Validate(paramList.ToArray());

            Assert.Null(errors);
            Assert.False(form.IsValid());
        }


        [Fact]
        public void ValidateMultipleFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.False(form.IsValid());

            var paramList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(MxFormProc.ProgramErrorKey, ""),
                new KeyValuePair<string, object>(IndexFormProc.HasSymptomsKey, "gg"),
                new KeyValuePair<string, object>(IndexFormProc.StartIsolationKey, "01-01-xx 17:35"),
                new KeyValuePair<string, object>(IndexFormProc.StartSymptomsKey, "01-01-2020 5:35 PM"),
            };

            var errors = form.Validate(paramList.ToArray());

            Assert.NotNull(errors);
            Assert.True(errors.Count == 3);
            Assert.True(errors.TryGetValue(IndexFormProc.HasSymptomsKey, out var errHasSymptoms));
            Assert.StartsWith("Please enter either 'yes' or 'no'", errHasSymptoms);
            Assert.True(errors.TryGetValue(IndexFormProc.StartIsolationKey, out var errStartIsolate));
            Assert.StartsWith("Please try again with a valid date/time", errStartIsolate);
            Assert.True(errors.TryGetValue(IndexFormProc.StartSymptomsKey, out var errStartSymptoms));
            Assert.StartsWith("You must set the start of your self-isolation", errStartSymptoms);

            Assert.False(form.IsValid());
        }

        [Fact]
        public void BasicTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Null(form.ValidateHasSymptoms("no"));
            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));
            Assert.Null(form.ValidateStartSymptoms("01-01-2020 5:35 PM"));

            Assert.True(form.IsValid());
            Assert.False(form.HasSymptoms);

            var tim = new LocalDateTime(2020, 01, 1, 17, 35, 00).InZoneStrictly(_zoneGmt).ToInstant();
            Assert.Equal(tim, form.StartIsolation);
            Assert.Equal(tim, form.StartSymptoms);

        }

        [Fact]
        public void HasSymptomsYesTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            var tim = new LocalDateTime(2020, 01, 1, 17, 35, 00).InZoneStrictly(_zoneGmt).ToInstant();


            Assert.Null(form.ValidateHasSymptoms("yes"));
            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));
            Assert.Null(form.ValidateStartSymptoms("01-01-2020 5:35 PM"));

            Assert.True(form.IsValid());
            Assert.True(form.HasSymptoms);
            Assert.Equal(tim, form.StartIsolation);
            Assert.Equal(tim, form.StartSymptoms);
        }

        [Fact]
        public void HasSymptomsNoTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            var tim = new LocalDateTime(2020, 01, 1, 17, 35, 00).InZoneStrictly(_zoneGmt).ToInstant();


            Assert.Null(form.ValidateHasSymptoms("no"));
            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));
            Assert.Null(form.ValidateStartSymptoms("01-01-2020 5:35 PM"));

            Assert.True(form.IsValid());
            Assert.False(form.HasSymptoms);
            Assert.Equal(tim, form.StartIsolation);
            Assert.Equal(tim, form.StartSymptoms);
        }

        [Fact]
        public void NullSymptomsTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            var tim = new LocalDateTime(2020, 01, 1, 17, 35, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));
            Assert.Null(form.ValidateStartSymptoms(null));

            Assert.True(form.IsValid());
            Assert.Equal(tim, form.StartIsolation);
            Assert.Null(form.StartSymptoms);
        }

        [Fact]
        public void SymptomsNotValidatedTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            var tim = new LocalDateTime(2020, 01, 1, 17, 35, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));

            Assert.True(form.IsValid());
            Assert.Equal(tim, form.StartIsolation);
            Assert.Null(form.StartSymptoms);

        }

        [Fact]
        public void EmptySymptomsTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            var tim = new LocalDateTime(2020, 01, 1, 17, 35, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));
            Assert.Null(form.ValidateStartSymptoms(""));

            Assert.True(form.IsValid());
            Assert.Equal(tim, form.StartIsolation);
            Assert.Null(form.StartSymptoms);

        }

        [Fact]
        public void SymptomsValidatedBeforeIsolationTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            var tim = new LocalDateTime(2020, 01, 1, 17, 35, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Contains("You must set the start of your self-isolation before giving the start of your symptoms", form.ValidateStartSymptoms("01-01-2020 5:01 PM"));
            Assert.False(form.IsValid());

            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));
            Assert.Null(form.ValidateStartSymptoms("01-01-2020 5:35 PM"));

            Assert.True(form.IsValid());
            Assert.Equal(tim, form.StartIsolation);
            Assert.Equal(tim, form.StartSymptoms);
        }

        [Fact]
        public void StartSymptomsAfterCurrentTimeFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:35 PM"));
            Assert.Contains("This value is after the current time", form.ValidateStartSymptoms("01-01-2050 5:01 PM"));

            Assert.False(form.IsValid());

        }

        [Fact]
        public void StartSymptomsBeforeSelfIsolationFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Null(form.ValidateStartIsolation("01-01-2020 5:01 PM"));
            Assert.Contains("This value is before the start of your self-isolation", form.ValidateStartSymptoms("01-01-2019 5:01 PM"));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void HasSymptomsNullFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);
            Assert.Equal("This value is required", form.ValidateHasSymptoms(null));
            Assert.False(form.IsValid());
        }

        [Fact]
        public void HasSymptomsEmptyFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Equal("This value is required", form.ValidateHasSymptoms(""));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void HasSymptomsInvalidFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Equal("Please enter either 'yes' or 'no'", form.ValidateHasSymptoms("gg"));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void StartIsolationEmptyFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Equal("This value is required", form.ValidateStartIsolation(""));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void StartIsolationNullFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Equal("This value is required", form.ValidateStartIsolation(null));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void StartIsolationInvalidDateFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Contains("Please try again with a valid date/time like", form.ValidateStartIsolation("01-01-20xx 17:01:59"));

            Assert.False(form.IsValid());

        }

        [Fact]
        public void StartIsolationAfterCurrentTimeFailTest()
        {
            var zones = new AppSupportedTimeZones();

            var form = new IndexFormProc(_clock, zones.GetTzDbName("GMT"), "en-GB", false);

            Assert.Contains("This value is after the current time", form.ValidateStartIsolation("01-01-2050 5:01 PM"));

            Assert.False(form.IsValid());
        }
    }
}
