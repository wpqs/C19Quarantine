using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace C19QCalcLib
{
    public sealed class MxLocalizedText
    {
        private readonly Object _lock = new Object();
        private readonly Dictionary<string, ResourceManager> _resourceManagers;

        // ReSharper disable once InconsistentNaming
        private static readonly MxLocalizedText _instance = new MxLocalizedText();
        static MxLocalizedText() { }

        // ReSharper disable once ConvertToAutoProperty
        public static MxLocalizedText Instance
        {
            get
            {
                // ReSharper disable once ArrangeAccessorOwnerBody
                return _instance;  
            }
        }
        private MxLocalizedText()
        {
            _resourceManagers = new Dictionary<string, ResourceManager>();
        }

        private ResourceManager GetResourceManager(Assembly asm)
        {
            ResourceManager rc = null;
            try
            {
                lock (_lock)
                {
                    if (asm != null)
                    {
                        if (_resourceManagers.TryGetValue(asm.GetName().Name + ".Properties.Resources", out var result))
                            rc = result;
                        else
                        {
                            var baseName = asm.GetName().Name + ".Properties.Resources";
                            var rm = new ResourceManager(baseName, asm);
                            _resourceManagers.Add(baseName, rm);
                            rc = rm;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ignore
            }
            return rc;
        }
        
        public string GetText(Assembly asm, string resourceName, CultureInfo culture=null)
        {
            var rc = "[not found]";
            try
            {
                var rm = GetResourceManager(asm);
                if (rm != null)
                    rc = rm.GetString(resourceName ?? "NotFound", culture);
            }
            catch (Exception)
            {
                //ignore
            }
            return rc;
        }
    }
}
