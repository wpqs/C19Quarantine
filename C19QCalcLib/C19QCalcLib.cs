using System.Reflection;

// ReSharper disable once IdentifierTypo
namespace C19QCalcLib
{
    // ReSharper disable once IdentifierTypo
    public class C19QCalcLib
    {
        public static string GetVersion()
        {
            return typeof(C19QCalcLib).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
        }

        public static Assembly GetAsm()
        {
            return typeof(C19QCalcLib).Assembly;
        }
    }
}
