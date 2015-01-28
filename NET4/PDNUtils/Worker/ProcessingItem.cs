namespace PDNUtils.Worker
{
    public class ProcessingItem<T>
    {
        public long Id { get; set; }
        public long Started { get; set; }
        public T Value { get; set; }
    }
}