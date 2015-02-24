using System;

namespace Replacer.Instruments
{
    public class GuidInstrument : Instrument
    {

        private Func<string, string> guidProvider;

        public GuidInstrument():this(GetGuid)
        {
        }

        internal GuidInstrument(Func<string, string> guidProvider)
        {
            this.guidProvider = guidProvider;
        }

        public string Instrument(string original)
        {
            return guidProvider(original);
        }

        internal static string GetGuid(string original)
        {
            return Guid.NewGuid().ToString();
        }
    }
}