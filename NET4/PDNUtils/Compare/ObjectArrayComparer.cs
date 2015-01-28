using System.Collections.Generic;
using PDNUtils.Runner.Attributes;

namespace PDNUtils.Compare
{
    public class ObjectArrayComparer : IEqualityComparer<object[]>
    {
        public bool Equals(object[] l1, object[] l2)
        {
            if (l1.Length != l2.Length)
                return false;
            for (int i = 0; i < l1.Length; i++)
            {
                if (!object.Equals(l1[i], l2[i]))
                    return false;
            }
            return true;
        }

        public int GetHashCode(object[] obj)
        {
            if (obj.Length == 0)
            {
                return obj.GetHashCode();
            }
            int code = obj[0].GetHashCode();
            for (int i = 1; i < obj.Length; i++)
            {
                if (obj[i] != null)
                {
                    code ^= obj[i].GetHashCode();
                }
            }
            return code;
        }
    }
}