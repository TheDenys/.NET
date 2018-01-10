using System;

namespace CompressLib
{
    public class OrderedItem<T>
    {
        public long Order { get; }
        public T Value { get; }

        public OrderedItem(long order, T value)
        {
            if (order < 0) throw new ArgumentOutOfRangeException(nameof(order));
            Order = order;
            Value = value;
        }
    }
}