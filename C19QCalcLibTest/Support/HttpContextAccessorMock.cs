using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace C19QCalcLibTest.Support
{
    //https://stackoverflow.com/questions/45959605/inspect-defaulthttpcontext-body-in-unit-test-situation#comment78887946_45960766
    //https://stackoverflow.com/questions/38475501/how-to-add-a-cookie-to-defaulthttpcontext
    class HttpContextAccessorMock : IHttpContextAccessor
    {
        public HttpContextAccessorMock()
        {
            Cookies = new List<string>();
        }
        public void AddCookie(string name, string value) { Cookies.Add($"{name}={value}"); }
        public void ClearCookies() {  Cookies.Clear();}
        private List<string> Cookies { set; get; }
        public HttpContext HttpContext
        {
            get
            {
                var context = new DefaultHttpContext();
                context.Request.Headers["Cookie"] = Cookies.ToArray();
                return context;
            }
            set { throw new NotImplementedException(); }
        }

    }
}
