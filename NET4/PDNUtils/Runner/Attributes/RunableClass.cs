using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDNUtils.Runner.Attributes
{
    /// <summary>
    /// attribute shows if class has methods that can run
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RunableClass:Attribute
    {
    }
}
