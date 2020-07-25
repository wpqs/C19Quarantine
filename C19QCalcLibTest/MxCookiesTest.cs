﻿using C19QCalcLib;
using C19QCalcLibTest.Support;
using Xunit;

namespace C19QCalcLibTest
{
    public class MxCookiesTest
    {
        private readonly MxCookies _cookies;

        public MxCookiesTest()
        {
            _cookies = new MxCookies(new HttpContextAccessorMock());
        }

        [Fact]
        public void Test()
        {
            var mockAccessor = (HttpContextAccessorMock) _cookies.Accessor;
            mockAccessor.AddCookie(".AspNetCore.Culture", "c=en|uic=en-GB");

            Assert.Equal("c=en|uic=en-GB", _cookies.GetValue(".AspNetCore.Culture"));

        }


    }
}