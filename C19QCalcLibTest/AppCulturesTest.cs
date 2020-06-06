using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class AppCulturesTest
    {
        [Fact]
        public void DefaultSelectedTest()
        {
            var cultures = new AppCultures();

            Assert.Equal(AppCultures.EnGb, cultures.Selected);
        }
    }
}
