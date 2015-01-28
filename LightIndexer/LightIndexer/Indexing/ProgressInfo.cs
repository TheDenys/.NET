namespace LightIndexer.Indexing
{
    public struct ProgressInfo
    {
        public readonly long CountScanned;
        public readonly long CountTotal;

        public ProgressInfo(long countScanned, long countTotal)
        {
            CountScanned = countScanned;
            CountTotal = countTotal != -1 ? countTotal : long.MaxValue;
        }
    }
}