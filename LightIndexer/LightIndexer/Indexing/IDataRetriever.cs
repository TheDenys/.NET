namespace LightIndexer.Indexing
{
    public interface IDataRetriever<out T>
    {
        int Count { get; }
        T GetItem(int rowIndex);
    }
}