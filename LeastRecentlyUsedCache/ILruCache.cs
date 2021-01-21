namespace LeastRecentlyUsedCache
{
    public interface ILruCache<TKey, TValue>
    {
        void Set(TKey key,TValue value);
        TValue Get(TKey key);
        int? ThreshHoldSize { get; set; }

        event EvictedNotification EvictionOccured;
    }
}