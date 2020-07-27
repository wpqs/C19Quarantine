using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using C19QCalcLibTest.Support;
using NodaTime;
using Xunit;

namespace C19QCalcLibTest
{
    public class MxLocalizedTextTest
    {
        private readonly C19QCalcLib.C19QCalcLib _lib;
        private readonly DateTimeZone _zoneGmt;
        public MxLocalizedTextTest()
        {
            _lib = new C19QCalcLib.C19QCalcLib(new HttpContextAccessorMock());
            _zoneGmt = DateTimeZoneProviders.Tzdb["Europe/London"];
        }

        [Fact]
        public void InterpolatedString()
        {
            var xchgRate = 1.236;
            Assert.Equal("today's rate = 1.24", $"today's rate ={xchgRate,5:N2}");
        }

        [Fact]
        public void FormattableStringTest()
        {
            var xchgRate = 1.236;
            FormattableString str = $"today's rate ={xchgRate,5:N2}";
            var cultureGb = CultureInfo.GetCultureInfo("en-GB");
            var cultureFr = CultureInfo.GetCultureInfo("fr");

            Assert.Equal("today's rate = 1.24", str.ToString(cultureGb) );
            Assert.Equal("today's rate = 1,24", str.ToString(cultureFr));


            Assert.Equal("today's rate = 1.24", str.ToString(CultureInfo.InvariantCulture));
            Assert.Equal("today's rate = 1.24", FormattableString.Invariant($"today's rate ={xchgRate,5:N2}"));
        }


        [Fact]
        [SuppressMessage("ReSharper", "UseStringInterpolation")]
        public void VerbatimStringTest()
        {
            var x = 3;
            var y = 5;

            string text1 = $"x={x}, y={y}";

            string text2 = string.Format("x={0}, y={1}", x, y);

            Assert.Equal(text1, text2);

            Assert.Equal("c:\\test.txt", @"c:\test.txt");

            var cmd1 = @"SELECT CITY, ZipCode
                        FROM Address
                        WHERE Country = 'UK'";
            Assert.Equal($"SELECT CITY, ZipCode{Environment.NewLine}                        FROM Address{Environment.NewLine}                        WHERE Country = 'UK'", cmd1);
            
            var cmd2 = "SELECT CITY, ZipCode " +
                        "FROM Address " +
                        "WHERE Country = 'UK'";
            Assert.Equal($"SELECT CITY, ZipCode FROM Address WHERE Country = 'UK'", cmd2);

        }

        [Fact]
        [SuppressMessage("ReSharper", "UseStringInterpolation")]
        public void StandardFormatSpecifierTest()
        {
            Assert.Equal("12,345,679", string.Format("{0:N0}", 12345678.9));
            Assert.Equal("12,345,678.9", string.Format("{0:N1}", 12345678.9));
            Assert.Equal("12,345,678.90", string.Format("{0:N2}", 12345678.9));

            Assert.Equal("46%", string.Format("{0:P0}", 0.4567));
            Assert.Equal("45.7%", string.Format("{0:P1}", 0.4567));
            Assert.Equal("45.67%", string.Format("{0:P2}", 0.4567));

            Assert.Equal("12345678", string.Format("{0:G8}", 12345678));
            Assert.Equal("1.234568E+07", string.Format("{0:G7}", 12345678));

            Assert.Equal("012345678", string.Format("{0:D9}", 12345678));
            Assert.Equal("12345678", string.Format("{0:D8}", 12345678));
            Assert.Equal("12345678", string.Format("{0:D7}", 12345678));

            Assert.Equal("1.234567800e+007", string.Format("{0:e9}", 12345678));
            Assert.Equal("1.23456780e+007", string.Format("{0:e8}", 12345678));
            Assert.Equal("1.2345678e+007", string.Format("{0:e7}", 12345678));

            Assert.Equal("12345679", string.Format("{0:F0}", 12345678.9));
            Assert.Equal("12345678.9", string.Format("{0:F1}", 12345678.9));
            Assert.Equal("12345678.90", string.Format("{0:F2}", 12345678.9));

            Assert.Equal("00BC614E", string.Format("{0:X8}", 12345678));
            Assert.Equal("0bc614e", string.Format("{0:x7}", 12345678));
            Assert.Equal("bc614e", string.Format("{0:x6}", 12345678));
            Assert.Equal("bc614e", string.Format("{0:x5}", 12345678));

            Assert.Equal("123", string.Format("{0:G5}", 123));
        }

        [Fact]
        [SuppressMessage("ReSharper", "UseStringInterpolation")]
        public void CustomFormatSpecifierTest()
        {
                        //# - extend number of digits as needed
            Assert.Equal("1234", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#}", 01234));                        //display number using as many digits as required
            Assert.Equal("12345678", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#}", 12345678));                 //extend to number of digits as required
            Assert.Equal("12345679", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#}", 12345678.56));              //round as needed
                                             //negative 
            Assert.Equal("-1234", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#}", -01234));                      //display number using as many digits as required
                                             //fractions
            Assert.Equal("1234", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#.#}", 01234));                      //no fraction, so don't display digit
            Assert.Equal("1234.6", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#.#}", 01234.556));                //round fraction to one digit
            Assert.Equal("1234.56", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#.##}", 01234.556));              //round fraction to two digits
                                            //group digits
            Assert.Equal("12,345,678.6", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#,#.#}", 12345678.566));     //separate groups of three digits with commas (for en-GB)
            Assert.Equal("1,23,45,678.57", string.Format(CultureInfo.CreateSpecificCulture("hi-IN"), "{0:#,#.##}", 12345678.566));  //separate groups of two digits with commas (for hi-IN)
                                            //divide by thousand    
            Assert.Equal("12,345.7", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#,#,.#}", 12345678.566));        //divide by 1000 and round fraction to one digit
            Assert.Equal("12.3", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#,#,,.#}", 12345678.566));           //divide by 1000,000 and round fraction to one digit
                                            //percentage
            Assert.Equal("46%", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#%}", 0.4566));                       //multiply by 100 
            Assert.Equal("5.7%", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#.#%}", 0.0566));                    //multiply by 100 and round fraction to one digit
            Assert.Equal("%", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#%}", 0.00366));                        //round fraction 
            Assert.Equal("0%", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.#%}", 0.00022));                     //round fraction and display zero if needed
            Assert.Equal("0.4%", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.#%}", 0.00366));                   //round fraction 
            
                        //0 - pad with leading zeros
            Assert.Equal("001234", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:000000}", 1234));             //minimum 6 digit number, padded with leading zeros as required
            Assert.Equal("12345678", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:000000}", 12345678));            //extend to number of digits as required
                                            //fractions
            Assert.Equal("001235", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:000000}", 1234.56));               //round integer
            Assert.Equal("001234.57", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:000000.00}", 1234.566));        //round fraction
            Assert.Equal("001234.500", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:000000.000}", 1234.5));        //pad with trailing zeros as required
                                            //group
            Assert.Equal("001,234.500", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:00000,0.000}", 1234.5));      //separate groups of three digits with commas (for en-GB)
            Assert.Equal("0,01,234.500", string.Format(CultureInfo.CreateSpecificCulture("hi-IN"), "{0:00000,0.000}", 1234.5));     //separate groups of two digits with commas (for hi-IN)
                                            //percent
            Assert.Equal("05.7%", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:00.#%}", 0.0566));                  //multiply by 100 add leading zero as required and round fraction to one digit

                        //# - fixed length numbers (telephone)
            Assert.Equal("(123) 456-7890", string.Format(CultureInfo.CreateSpecificCulture("en-GB"),  "{0:(###) ###-####}", 1234567890));   //display fixed length number in given format 
            Assert.Equal("() 56-7890", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:(###) ###-####}", 567890));                    //number less than expected length
            Assert.Equal("(123456) 789-1234", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:(###) ###-####}", 1234567891234));      //number more than expected length - extended

                        //e - scientific format
            Assert.Equal("1.235e7", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e0}", 12345678.566));
            Assert.Equal("-1.235e7", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e0}", -12345678.566));
            Assert.Equal("5.66e-5", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e0}", 0.0000566));
            Assert.Equal("-5.66e-5", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e0}", -0.0000566));

            Assert.Equal("1.235e+7", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e+0}", 12345678.566));
            Assert.Equal("-1.235e+7", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e+0}", -12345678.566));
            Assert.Equal("5.66e-5", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e+0}", 0.0000566));
            Assert.Equal("-5.66e-5", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:0.###e+0}", -0.0000566));

                        //; - section formatter (first for +ve numbers, second for -vs numbers, third for zero values
            Assert.Equal("1234", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:##;(##);**Zero**}", 1234));
            Assert.Equal("(1234)", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:##;(##);**Zero**}", -1234));
            Assert.Equal("**Zero**", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:##;(##);**Zero**}", 0));

                        //\ - escape character
            Assert.Equal(".0566%", string.Format(CultureInfo.CreateSpecificCulture("en-GB"), "{0:#.####\\%}", 0.0566));                 //treat % as literal

        }

        [Fact]
        public void GetTextSimpleTest()
        {
            var mockAccessor = (HttpContextAccessorMock)_lib.HttpContextAccessor;
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=en-GB|uic=en-GB");

            Assert.Equal("Welcome - en-GB", _lib.GetText("Welcome"));
            Assert.Equal("Welcome - en-US", _lib.GetText("Welcome", "en-US"));
        }

        [Fact]
        public void GetTextNotFoundFailTest()
        {
            var mockAccessor = (HttpContextAccessorMock)_lib.HttpContextAccessor;
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=en-GB|uic=en-GB");

            Assert.Equal("[Not Found]", _lib.GetText("WelcomeXXX"));
        }

        [Fact]
        public void GetTextFormatTest()
        {
            var winterTime = new LocalDateTime(2020, 01, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Equal("Welcome - en - Fred", _lib.GetText("WelcomeP1", null, "Fred"));
            Assert.Equal("Welcome - en - Fred, 30-01-2020 5:45 PM", _lib.GetText("WelcomeP2", null, "Fred", _lib.InstantToString(winterTime)));
            Assert.Equal("Welcome - en - Fred, 30-01-2020 5:45 PM - 1,000.00", _lib.GetText("WelcomeP3", null, "Fred", _lib.InstantToString(winterTime), 1000));
        }

        [Fact]
        public void GetTextFormatFrChTest()
        {
            var winterTime = new LocalDateTime(2020, 01, 30, 17, 45, 00).InZoneStrictly(_zoneGmt).ToInstant();

            Assert.Equal("Welcome - fr-CH - Fred", _lib.GetText("WelcomeP1", "fr-CH", "Fred"));
            Assert.Equal("Welcome - fr-CH - Fred, 30.01.2020 17:45", _lib.GetText("WelcomeP2", "fr-CH", "Fred", _lib.InstantToString(winterTime, "fr-CH")));
            Assert.Equal("Welcome - fr-CH - Fred, 30.01.2020 17:45 - 10 000,00", _lib.GetText("WelcomeP3", "fr-CH", "Fred", _lib.InstantToString(winterTime, "fr-CH"), 10000));
        }
    }
}