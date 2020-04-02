using System.Collections.Generic;
using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class CalcUkTest
    {
        [Fact]
        public void GetDaysInQuarantineBasicTest()
        {
            var household = new List<Person>();
            household.Add(new Person());

            var calc = new CalcUk();
             
            Assert.Equal(-1, calc.GetDaysInQuarantine(household.ToArray()));

        }
    }
}
