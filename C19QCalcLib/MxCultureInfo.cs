using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace C19QCalcLib
{
    public sealed class MxCultureInfo
    {
        public enum FormatType
        {
            DateTime = 0,
            Date = 1,
            Time = 2,
            Verbose = 3,
            Machine = 4
        }

        // ReSharper disable once InconsistentNaming
        private static readonly MxCultureInfo _instance = new MxCultureInfo();
        private readonly Dictionary<string, CultureInfo> _cultures;
        static MxCultureInfo() { }
        private MxCultureInfo() { _cultures = new Dictionary<string, CultureInfo>(); }
        public static MxCultureInfo Instance { get { return _instance; } }

        private Object _lock = new Object();
        public CultureInfo GetCultureInfo(string cultureName = null)
        {
            CultureInfo rc = null;
            try
            {
               lock (_lock)
               {
                   var name = cultureName ?? Thread.CurrentThread.CurrentCulture.Name;
                   if (_cultures.TryGetValue(name, out var result))
                       rc = result;
                   else
                   {
                       var cultureInfo = CultureInfo.GetCultureInfo(name);
                       if (name == "en-GB")
                       {
                           cultureInfo = CultureInfo.CreateSpecificCulture(name);
                           cultureInfo.DateTimeFormat.LongDatePattern = "dddd, d MMMM yyyy";
                           cultureInfo.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy"; //format specifier "G" uses ShortDatePattern to render it's date component so use yyyy to keep consistency with other cultures
                           cultureInfo.DateTimeFormat.ShortTimePattern = "h:mm tt";
                       }

                       _cultures.Add(name, cultureInfo);
                       rc = cultureInfo;
                   }
               }
            }
            catch (Exception)
            {
                //ignore
            }
            return rc;
        }

        public static string GetFormatSpecifier(FormatType formatType, bool longFormat)
        {
            string rc;

            if (formatType == FormatType.Date)
                rc = longFormat ? "D" : "d";  //en-GB D=Sunday, 29 March 2020 d=29-03-2020 
            else if (formatType == FormatType.Time)
                rc = longFormat ? "T" : "t"; //en-GB: T=00:59:59 t=12:59 AM 
            else if (formatType == FormatType.Verbose)
                rc = longFormat ? "F" : "f";     //en-GB: F=Sunday, 29 March 2020 00:59:59 f=Sunday, 29 March 2020 12:59 AM 
            else if (formatType == FormatType.Machine)
                rc = longFormat ? "r" : "s";     //all cultures: r=2020-03-29T00:59:59.000000000 (ISO) s=2020-03-29T00:59:59
            else                 //FormatType.DateTime
                rc = longFormat ? "G" : "g";     //en-GB: G=29-03-2020 00:59:59  g=29-03-2020 12:59 AM

            return rc;
        }
    }
}
