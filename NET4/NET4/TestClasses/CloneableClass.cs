using System;

namespace NET4.TestClasses
{
    public class CloneableClass : ICloneable
    {
        public int Value { get; set; }

        public bool IsClone { get; set; }

        public CloneableClass(int value)
        {
            this.Value = value;
            IsClone = false;
        }

        #region Implementation of ICloneable

        public object Clone()
        {
            CloneableClass cc = new CloneableClass(Value);
            cc.IsClone = true;
            return cc;
        }

        #endregion
    }
}