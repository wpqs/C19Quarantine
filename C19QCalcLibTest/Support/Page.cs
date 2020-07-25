using System.Reflection;
using C19QCalcLib;
using Microsoft.AspNetCore.Http;

namespace C19QCalcLibTest.Support
{
    public class Page : MxLib
    {
        public Page(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
           
        }

        public override Assembly GetResourcesAsm()
        {
            return typeof(C19QCalcLib.C19QCalcLib).Assembly;
        }
    }
}
