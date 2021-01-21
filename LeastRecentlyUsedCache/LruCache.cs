using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace LeastRecentlyUsedCache
{
    
    public sealed class LruCache<TKey,TValue> : ILruCache<TKey, TValue>
    {
        private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey,TValue>>> _cacheMap;
        private LinkedList<KeyValuePair<TKey,TValue>> _linkedList;
        private static LruCache<TKey, TValue> _instance;
        private static readonly object padlock = new object();
        private const int InitialThresHoldSize = 10;
        public static LruCache<TKey, TValue> Instance
        {
            get
            {
                lock (padlock)
                {
                    return _instance ?? (_instance = new LruCache<TKey, TValue>());
                }
            }
        }
        private LruCache()
        {
            ThreshHoldSize = ConfigurationManager.AppSettings["ThreshHoldSize"] != null
                ? int.Parse(ConfigurationManager.AppSettings["ThreshHoldSize"])
                : InitialThresHoldSize;
            _cacheMap = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
            _linkedList = new LinkedList<KeyValuePair<TKey, TValue>>();

        }


        public void Set(TKey key,TValue value)
        {
            // first we check for eviction if needed
            if (IsEvictionRequired(key))            
                EvictLeastRecentlyUsed();

            // now we check if dictionary contains key then we remove it from the linkedlist and from the dictionary
            if (_cacheMap.ContainsKey(key))
            {
                Remove(_cacheMap[key]);
            }
            // create a new node and add it to the start of the linkedlist to make it the most recently accessed item
            // and also add it to the dictionary
            LinkedListNode<KeyValuePair<TKey, TValue>> node = new LinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value));
            _linkedList.AddFirst(node);
            _cacheMap[key] = node;           
            
        }

        public TValue Get(TKey key)
        {
            if (_cacheMap.ContainsKey(key))
            {
                SetMostRecentlyAccessedItem(key);
                return _cacheMap[key].Value.Value;
            }

            return default(TValue);

        }

        public int? ThreshHoldSize { get; set; }
        public event EvictedNotification EvictionOccured;


        private void SetMostRecentlyAccessedItem(TKey key)
        {
            // we need to make the item that has been accessed recently to appear in top of the linked list , so we remove it from link list and then add it to the first position
            var lnNode = _cacheMap[key];
            _linkedList.Remove(lnNode);
            _linkedList.AddFirst(lnNode);
            
        }

        private bool IsEvictionRequired(TKey key)
        {
            return _linkedList.Count == ThreshHoldSize && !_cacheMap.ContainsKey(key);           
        }

        private void EvictLeastRecentlyUsed()
        {
            LinkedListNode<KeyValuePair<TKey, TValue>> leastAccessedNode = _linkedList.Last;
            Remove(leastAccessedNode);
            // invoke the eviction occured event here
            EvictionOccured?.Invoke();
        }
        private void Remove(LinkedListNode<KeyValuePair<TKey, TValue>> node)
        {           
            _cacheMap.Remove(node.Value.Key);            
            _linkedList.Remove(node);
        }

    }
}
