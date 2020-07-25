using System.Reflection;
using Microsoft.AspNetCore.Http;

// ReSharper disable once IdentifierTypo
namespace C19QCalcLib
{
    // ReSharper disable once IdentifierTypo
    public class C19QCalcLib : MxLib
    {
        public C19QCalcLib(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {

        }

        public static string GetVersion()
        {
            return typeof(C19QCalcLib).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
        }

        public override Assembly GetResourcesAsm()
        {
            return typeof(C19QCalcLib).Assembly;
        }
    }
}
