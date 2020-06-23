using C19QCalcLib;
using Xunit;

namespace C19QCalcLibTest
{
    public class AppSupportedCulturesTest
    {
        [Fact]
        public void DefaultSelectedTest()
        {
            var cultures = new AppSupportedCultures();

            Assert.Equal(AppSupportedCultures.En, cultures.Selected);
            Assert.Equal(AppSupportedCultures.En, cultures.GetDefaultCultureTab());
        }
    }
}
