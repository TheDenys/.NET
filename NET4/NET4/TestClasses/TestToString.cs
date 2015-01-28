using System;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    internal class TestToString
    {

        private class WithFormat : IFormattable
        {
            private string name;

            private int price;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public int Price
            {
                get { return price; }
                set { price = value; }
            }

            public string ToString(string format, IFormatProvider formatProvider)
            {
                if (formatProvider != null)
                {
                    ICustomFormatter formatter = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;

                    if (formatter != null)
                    {
                        return formatter.Format(format, this, formatProvider);
                    }
                }

                switch (format)
                {
                    case "p":
                    case "P":
                        return this.Price.ToString();
                    case "n":
                    case "G":
                    default:
                        return this.Name;
                }
            }
        }

        private class MyFormatter : ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                WithFormat wf = arg as WithFormat;

                if (wf != null)
                {
                    return string.Format(format, wf.Name, wf.Price);
                }

                throw new ArgumentException();
            }
        }

        private class MyFormatProvider : IFormatProvider
        {
            public object GetFormat(Type formatType)
            {
                if (formatType == typeof(WithFormat))
                {
                    return new MyFormatter();
                }

                throw new ArgumentOutOfRangeException();
            }
        }

        [Run(0)]
        protected void TestFormats()
        {
            WithFormat wf = new WithFormat { Name = "Vogue", Price = 999999 };

            ConsolePrint.print(wf);
            ConsolePrint.print(wf.ToString("name:'{0}', price:'{1}'", new MyFormatProvider()));
        }
    }
}