using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class UiValidationTest
    {
        [Fact]
        public void NoErrorWithSymptoms()
        {
            var validation = new UiValidation();
            Assert.Null(validation.ProcessForm("01-01-20 17:35", "01-01-20 17:35", "37.0", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void NoErrorNullSymptoms()
        {
            var validation = new UiValidation();
            Assert.Null(validation.ProcessForm("01-01-20 17:35", null, "37.0","GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void NoErrorEmptySymptoms()
        {
            var validation = new UiValidation();
            Assert.Null(validation.ProcessForm("01-01-20 17:35", "", "37.0", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void TemperatureEmpty()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 001", validation.ProcessForm("01-01-20 17:35", "01-01-20 17:35", "", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void TemperatureNull()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 001", validation.ProcessForm("01-01-20 17:35", "01-01-20 17:35", null, "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void TemperatureNotANumber()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 002", validation.ProcessForm("01-01-20 17:35", "01-01-20 17:35", "3x.0", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void TemperatureOutOfRangeHigh()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 003", validation.ProcessForm("01-01-20 17:35", "01-01-20 17:35", "43.1", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void TemperatureOutOfRangeLow()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 003", validation.ProcessForm("01-01-20 17:35", "01-01-20 17:35", "23.9", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void StartQuarantineEmpty()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 004", validation.ProcessForm("", "01-01-20 17:35", "39.9", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void StartQuarantineNull()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 004", validation.ProcessForm(null, "01-01-20 17:35", "39.9", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void SelfIsolationAfterCurrentTime()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 006", validation.ProcessForm("01-01-2050 17:01:59", null, "39.9", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void StartSymptomsInvalidDate()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 007", validation.ProcessForm("01-01-2020 17:01:59", "01-xx-2020 17:01:59", "39.9", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void StartSymptomsAfterCurrentTime()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 008", validation.ProcessForm("01-01-2020 17:01:59", "01-01-2050 17:01:59", "39.9", "GMT Standard Time", "en-GB"));
        }

        [Fact]
        public void StartSymptomsBeforeSelfIsolation()
        {
            var validation = new UiValidation();
            Assert.Contains("User error 009", validation.ProcessForm("01-01-2020 17:01:59", "01-01-2019 17:01:59", "39.9", "GMT Standard Time", "en-GB"));
        }
    }
}
