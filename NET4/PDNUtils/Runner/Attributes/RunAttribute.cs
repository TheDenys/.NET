using System;

namespace PDNUtils.Runner.Attributes
{
    /// <summary>
    /// attribute used by Runner class to mark methods which needs to be executed
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class RunAttribute : Attribute
    {
        /// <summary>
        /// Shows whether this method is enabled to execute
        /// </summary>
        public readonly bool Enabled;

        public RunAttribute()
        {
            Enabled = true;
        }

        public RunAttribute(bool enabled)
        {
            this.Enabled = enabled;
        }

        public RunAttribute(byte enabled)
        {
            this.Enabled = enabled != 0;
        }
    }
}