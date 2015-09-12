using System.Collections.Generic;

namespace PDNUtils.Help
{
    public class IListEqualityComparer<T> : IEqualityComparer<IList<T>>
    {
        public bool Equals(IList<T> x, IList<T> y)
        {
            if ((x == null && y == null) || ReferenceEquals(x, y)) return true;

            if (x != null)
            {
                if (y == null) return false;
                if (x.Count != y.Count) return false;

                for (int i = 0; i < x.Count; i++)
                {
                    if (!EqualityComparer<T>.Default.Equals(x[i], y[i]))
                        return false;
                }

                return true;
            }

            return false;
        }

        public int GetHashCode(IList<T> obj)
        {
            if (obj == null) return 0;

            int c = 0;

            unchecked
            {
                foreach (var o in obj)
                {
                    c = 37 * (o != null ? o.GetHashCode() : 0) + c;
                }
            }

            return c;
        }
    }
}