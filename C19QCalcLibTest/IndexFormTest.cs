using System;
using System.Collections.Generic;
using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class IndexFormTest
    {
        [Fact]
        public void ValidateTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");

            Assert.False(form.IsValid());

            var paramList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(MxFormProc.ProgramErrorKey, ""),
                new KeyValuePair<string, object>(IndexForm.HasSymptomsKey, "yes"),
                new KeyValuePair<string, object>(IndexForm.StartIsolationKey, "01-01-20 17:35"),
                new KeyValuePair<string, object>(IndexForm.StartSymptomsKey, "01-01-20 17:35"),
            };

            var errors = form.Validate(paramList.ToArray());

            Assert.NotNull(errors);
            Assert.Empty(errors);
            Assert.True(form.IsValid());

            Assert.True(form.HasSymptoms);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartSymptoms);
        }

        [Fact]
        public void ValidateMissingKeyFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");

            Assert.False(form.IsValid());

            var paramList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(MxFormProc.ProgramErrorKey, ""),
                new KeyValuePair<string, object>(IndexForm.StartSymptomsKey, "01-01-20 17:35"),
            };

            var errors = form.Validate(paramList.ToArray());

            Assert.Null(errors);
            Assert.False(form.IsValid());
        }


        [Fact]
        public void ValidateMultipleFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");

            Assert.False(form.IsValid());

            var paramList = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>(MxFormProc.ProgramErrorKey, ""),
                new KeyValuePair<string, object>(IndexForm.HasSymptomsKey, "gg"),
                new KeyValuePair<string, object>(IndexForm.StartIsolationKey, "01-01-xx 17:35"),
                new KeyValuePair<string, object>(IndexForm.StartSymptomsKey, "01-01-20 17:35"),
            };

            var errors = form.Validate(paramList.ToArray());

            Assert.NotNull(errors);
            Assert.True(errors.Count == 3);
            Assert.True(errors.TryGetValue(IndexForm.HasSymptomsKey, out var errHasSymptoms));
            Assert.StartsWith("Please enter either 'yes' or 'no'", errHasSymptoms);
            Assert.True(errors.TryGetValue(IndexForm.StartIsolationKey, out var errStartIsolate));
            Assert.StartsWith("Please try again with a valid date/time", errStartIsolate);
            Assert.True(errors.TryGetValue(IndexForm.StartSymptomsKey, out var errStartSymptoms));
            Assert.StartsWith("You must set the start of your self-isolation", errStartSymptoms);

            Assert.False(form.IsValid());
        }

        [Fact]
        public void BasicTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");

            Assert.Null(form.ValidateHasSymptoms("no"));
            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));
            Assert.Null(form.ValidateStartSymptoms("01-01-20 17:35"));

            Assert.True(form.IsValid());
            Assert.False(form.HasSymptoms);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartSymptoms);

        }

        [Fact]
        public void HasSymptomsYesTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");

            Assert.Null(form.ValidateHasSymptoms("yes"));
            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));
            Assert.Null(form.ValidateStartSymptoms("01-01-20 17:35"));

            Assert.True(form.IsValid());
            Assert.True(form.HasSymptoms);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartSymptoms);
        }

        [Fact]
        public void HasSymptomsNoTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");

            Assert.Null(form.ValidateHasSymptoms("no"));
            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));
            Assert.Null(form.ValidateStartSymptoms("01-01-20 17:35"));

            Assert.True(form.IsValid());
            Assert.False(form.HasSymptoms);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartSymptoms);
        }

        [Fact]
        public void NullSymptomsTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));
            Assert.Null(form.ValidateStartSymptoms(null));

            Assert.True(form.IsValid());
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Null(form.StartSymptoms);
        }

        [Fact]
        public void SymptomsNotValidatedTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));

            Assert.True(form.IsValid());
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Null(form.StartSymptoms);

        }

        [Fact]
        public void EmptySymptomsTest()
        {
           var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));
            Assert.Null(form.ValidateStartSymptoms(""));

            Assert.True(form.IsValid());
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Null(form.StartSymptoms);

        }

        [Fact]
        public void SymptomsValidatedBeforeIsolationTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Contains("You must set the start of your self-isolation before giving the start of your symptoms", form.ValidateStartSymptoms("01-01-2020 17:01:59"));
            Assert.False(form.IsValid());

            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));
            Assert.Null(form.ValidateStartSymptoms("01-01-2020 17:35"));

            Assert.True(form.IsValid());
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartIsolation);
            Assert.Equal(new DateTime(2020, 1, 1, 17, 35, 0), form.StartSymptoms);
        }

        [Fact]
        public void StartSymptomsAfterCurrentTimeFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Null(form.ValidateStartIsolation("01-01-20 17:35"));
            Assert.Contains("This value is after the current time", form.ValidateStartSymptoms("01-01-2050 17:01:59"));

            Assert.False(form.IsValid());

        }

        [Fact]
        public void StartSymptomsBeforeSelfIsolationFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Null(form.ValidateStartIsolation("01-01-2020 17:01:59"));
            Assert.Contains("This value is before the start of your self-isolation", form.ValidateStartSymptoms("01-01-2019 17:01:59"));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void HasSymptomsNullFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Equal("This value is required", form.ValidateHasSymptoms(null));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void HasSymptomsEmptyFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Equal("This value is required", form.ValidateHasSymptoms(""));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void HasSymptomsInvalidFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Equal("Please enter either 'yes' or 'no'", form.ValidateHasSymptoms("gg"));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void StartIsolationEmptyFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Equal("This value is required", form.ValidateStartIsolation(""));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void StartIsolationNullFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Equal("This value is required", form.ValidateStartIsolation(null));

            Assert.False(form.IsValid());
        }

        [Fact]
        public void StartIsolationInvalidDateFailTest()
        {
           
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Contains("Please try again with a valid date/time like", form.ValidateStartIsolation("01-01-20xx 17:01:59"));

            Assert.False(form.IsValid());

        }

        [Fact]
        public void StartIsolationAfterCurrentTimeFailTest()
        {
            var form = new IndexForm("GMT Standard Time", "en-GB");
            Assert.Contains("This value is after the current time", form.ValidateStartIsolation("01-01-2050 17:01:59"));

            Assert.False(form.IsValid());
        }
    }
}
