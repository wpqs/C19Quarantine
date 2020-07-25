using C19QCalcLibTest.Support;
using Xunit;

namespace C19QCalcLibTest
{
    public class C19QCalcLibTest
    {
        [Fact]
        public void GetVersionTest()
        {
            Assert.Equal("1.3.30.0", C19QCalcLib.C19QCalcLib.GetVersion());
        }

        [Fact]
        public void GetAsmTest()
        {
            var lib = new C19QCalcLib.C19QCalcLib(new HttpContextAccessorMock());
            Assert.StartsWith("C19QCalcLib",  lib.GetResourcesAsm().FullName);
        }
    }
}