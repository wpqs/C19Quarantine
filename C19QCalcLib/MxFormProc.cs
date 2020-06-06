using System.Collections.Generic;

namespace C19QCalcLib
{
    public abstract class MxFormProc
    {
        public const string ProgramErrorMsg = "Program Error";
        public const string ProgramErrorKey = "ProgramError";

        protected readonly Dictionary<string, string> Errors;
        protected readonly List<string> Keys;

        public abstract Dictionary<string, string> Validate(KeyValuePair<string, object>[] props);
        public abstract bool IsValid();

        protected MxFormProc()
        {
            Errors = new Dictionary<string, string>();
            Keys = new List<string>();
        }

        protected virtual bool ValidatePropKeys(KeyValuePair<string, object>[] props)
        {
            var rc = false;

            if (props?.Length > 1)
            {
                foreach (var prop in props)
                    Keys.Add(prop.Key);

                if (Keys.Contains(ProgramErrorKey))
                    rc = true;
            }
            return rc;
        }

        protected void AddError(string propName, string error)
        {
            if ((string.IsNullOrEmpty(propName) == false) && (string.IsNullOrEmpty(error) == false))
            {
                var programError = error.Contains(ProgramErrorMsg);
                if ((programError && (Errors.ContainsKey(ProgramErrorKey) == false)) || ((programError == false) && (Errors.ContainsKey(propName) == false)))
                    Errors.Add(programError ? ProgramErrorKey : propName, error);
            }
        }
    }
}
