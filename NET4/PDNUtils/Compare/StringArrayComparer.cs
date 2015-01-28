using System.Collections.Generic;

namespace PDNUtils.Compare
{
    public class StringArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] l1, string[] l2)
        {
            if (l1.Length != l2.Length)
                return false;
            for (int i = 0; i < l1.Length; i++)
            {
                if (l1[i] != l2[i])
                    return false;
            }
            return true;
        }

        public int GetHashCode(string[] obj)
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