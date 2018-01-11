using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET4.MultiPatternSearch
{
    abstract class PatternElement
    {
        public string Value { get; }

        protected PatternElement(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Value must contain at least one non-space character.", nameof(value));
            Value = value;
        }
    }

    class StringPatternElement : PatternElement
    {
        public StringPatternElement(string value) : base(value)
        {
        }
    }

    class WildcardPatternElement : PatternElement
    {
        public WildcardPatternElement(string name) : base(name)
        {
        }
    }
}
