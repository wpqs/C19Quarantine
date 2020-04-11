using System;
using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class UIValidationTest
    {
        [Fact]
        public void NoErrorWithSymptoms()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("01-01-20 17:35", "01-01-20 17:35", "37.0 oC"));
        }

        [Fact]
        public void NoErrorNullSymptoms()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("01-01-20 17:35", null, "37.0 oC"));
        }

        [Fact]
        public void DateTimeValidYear2020()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("01-01-2020 17:01", null, "39.9 oC"));
        }

        [Fact]
        public void DateTimeValidSeconds()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("01-01-2020 17:01:59", null, "39.9 oC"));
        }

        [Fact]
        public void DateTimeValidMonthAbrInChars()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("31-Jan-2020 17:01:59", null, "39.9 oC"));
        }

        [Fact]
        public void DateTimeValidMonthFullInChars()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("31-January-2020 17:01:59", null, "39.9 oC"));
        }


        [Fact]
        public void DateTimeValidForwardSlashSeparator()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("31/Jan/2020 17:01:59", null, "39.9 oC"));
        }

        [Fact]
        public void NoErrorEmptySymptoms()
        {
            var validation = new UiValidation();
            Assert.Null(validation.GetFormErrors("01-01-20 17:35", "", "37.0 oC"));
        }

        [Fact]
        public void TemperatureEmpty()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 001", validation.GetFormErrors("01-01-20 17:35", "01-01-20 17:35", ""));
        }

        [Fact]
        public void TemperatureNull()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 001", validation.GetFormErrors("01-01-20 17:35", "01-01-20 17:35", null));
        }

        [Fact]
        public void TemperatureNoSymbol()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 001", validation.GetFormErrors("01-01-20 17:35", "01-01-20 17:35", "39.0"));
        }

        [Fact]
        public void TemperatureNAN()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 002", validation.GetFormErrors("01-01-20 17:35", "01-01-20 17:35", "3x.0 oC"));
        }

        [Fact]
        public void TemperatureOutOfRangeHigh()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 003", validation.GetFormErrors("01-01-20 17:35", "01-01-20 17:35", "43.1 oC"));
        }

        [Fact]
        public void TemperatureOutOfRangeLow()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 003", validation.GetFormErrors("01-01-20 17:35", "01-01-20 17:35", "23.9 oC"));
        }

        [Fact]
        public void StartQuarantineEmpty()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 004", validation.GetFormErrors("", "01-01-20 17:35", "39.9 oC"));
        }

        [Fact]
        public void StartQuarantineNull()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 004", validation.GetFormErrors(null, "01-01-20 17:35", "39.9 oC"));
        }

        [Fact]
        public void DateTimeInvalidTime()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 005", validation.GetFormErrors("01-01-20 17:61", null, "39.9 oC"));
        }

        [Fact]
        public void DateTimeInvalidDay()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 005", validation.GetFormErrors("32-01-20 17:01", null, "39.9 oC"));
        }


        [Fact]
        public void DateTimeInvalidMonth()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 005", validation.GetFormErrors("31-13-20 17:01", null, "39.9 oC"));
        }

        [Fact]
        public void DateTimeInvalidYear()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 005", validation.GetFormErrors("01-01-xx 17:01", null, "39.9 oC"));
        }


        [Fact]
        public void DateTimeInValidBackslashSeparator()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 005", validation.GetFormErrors("31\\Jan\\2020 17:01:59", null, "39.9 oC"));
        }

        [Fact]
        public void SelfIsolationAfterCurrentTime()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 006", validation.GetFormErrors("01-01-2050 17:01:59", null, "39.9 oC"));
        }

        [Fact]
        public void StartSymptomsInvalidDate()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 007", validation.GetFormErrors("01-01-2020 17:01:59", "01-xx-2020 17:01:59", "39.9 oC"));
        }

        [Fact]
        public void StartSymptomsBeforeSelfIsolation()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 008", validation.GetFormErrors("01-01-2020 17:01:59", "01-01-2019 17:01:59", "39.9 oC"));
        }



    }
}
