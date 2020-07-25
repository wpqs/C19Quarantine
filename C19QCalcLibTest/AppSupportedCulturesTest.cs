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

            Assert.Equal(AppSupportedCultures.EnGb, cultures.Selected);
            Assert.Equal(AppSupportedCultures.EnGb, cultures.GetCultureTabForNeutralCulture());
            Assert.Equal(AppSupportedCultures.En, cultures.GetDefaultUiCultureTab());
        }

        [Fact]
        public void GetNearestMatchCultureTest()
        {
            var cultures = new AppSupportedCultures();

            Assert.Equal(AppSupportedCultures.EnGb, cultures.GetNearestMatch(null));
            Assert.Equal(AppSupportedCultures.EnGb, cultures.GetNearestMatch("xxx"));

            Assert.Equal(AppSupportedCultures.EnGb, cultures.GetNearestMatch("en"));
            Assert.Equal(AppSupportedCultures.EnGb, cultures.GetNearestMatch("en-GB"));
            Assert.Equal(AppSupportedCultures.EnGb, cultures.GetNearestMatch("en-US"));
            Assert.Equal(AppSupportedCultures.EnGb, cultures.GetNearestMatch("en-CH"));

            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr"));
            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr-CH"));
            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr-FR"));

            Assert.Equal(AppSupportedCultures.DeCh, cultures.GetNearestMatch("de"));
            Assert.Equal(AppSupportedCultures.DeCh, cultures.GetNearestMatch("de-CH"));
            Assert.Equal(AppSupportedCultures.DeCh, cultures.GetNearestMatch("de-FR"));

            Assert.Equal(AppSupportedCultures.ItCh, cultures.GetNearestMatch("it"));
            Assert.Equal(AppSupportedCultures.ItCh, cultures.GetNearestMatch("it-CH"));
            Assert.Equal(AppSupportedCultures.ItCh, cultures.GetNearestMatch("it-FR"));
        }

        [Fact]
        public void GetNearestMatchUiCultureTest()
        {
            var cultures = new AppSupportedCultures();

            Assert.Equal(AppSupportedCultures.En, cultures.GetNearestMatch("en", true));
            Assert.Equal(AppSupportedCultures.En, cultures.GetNearestMatch("en-GB", true));
            Assert.Equal(AppSupportedCultures.En, cultures.GetNearestMatch("en-US", true));
            Assert.Equal(AppSupportedCultures.En, cultures.GetNearestMatch("en-CH", true));
            
            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr-CH", true));
            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr-FR", true));

            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr", true));
            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr-CH", true));
            Assert.Equal(AppSupportedCultures.FrCh, cultures.GetNearestMatch("fr-FR", true));

            Assert.Equal(AppSupportedCultures.DeCh, cultures.GetNearestMatch("de", true));
            Assert.Equal(AppSupportedCultures.DeCh, cultures.GetNearestMatch("de-CH", true));
            Assert.Equal(AppSupportedCultures.DeCh, cultures.GetNearestMatch("de-FR", true));

            Assert.Equal(AppSupportedCultures.ItCh, cultures.GetNearestMatch("it", true));
            Assert.Equal(AppSupportedCultures.ItCh, cultures.GetNearestMatch("it-CH", true));
            Assert.Equal(AppSupportedCultures.ItCh, cultures.GetNearestMatch("it-FR", true));

        }
    }
}
