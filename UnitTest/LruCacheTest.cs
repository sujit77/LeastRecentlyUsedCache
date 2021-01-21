using LeastRecentlyUsedCache;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestFixture]
    public class LruCacheTest
    {
        [Test]
        public void GetNullItemTest()
        {
            //var cache = new LruCache<string, string>(int.MaxValue);
            var cache = LruCache<string, string>.Instance;
            var value = cache.Get("key");
            Assert.IsNull(value);
        }

        [Test]
        public void SetAndGetItemTest()
        {
            //var cache = new LruCache<string, string>(int.MaxValue);
            var cache = LruCache<string, string>.Instance;
            cache.Set("key", "100");
            var value = cache.Get("key");
            Assert.AreEqual("100", value);
        }

        [Test]
        public void SetTwoItemsWithSameKeyShouldReturnSecondValueTest()
        {
            //var cache = new LruCache<string, string>(int.MaxValue);
            var cache = LruCache<string, string>.Instance;

            cache.Set("key", "1002");
            cache.Set("key", "1003");
            var value = cache.Get("key");

            Assert.AreEqual("1003", value);
        }

        int _evictionEventOccuredCount = 0;
        [Test]
        public void SetMoreThanThresholdEvictionTest()
        {
            
            var cache = LruCache<string,string>.Instance;
            cache.ThreshHoldSize = 2;
            cache.EvictionOccured += Cache_EvictionOccured;
            cache.Set("key1", "1001");
            cache.Set("key2", "1002");
            cache.Set("key3", "1003");
            var value1 = cache.Get("key1");
            var value3 = cache.Get("key3");
            var value2 = cache.Get("key2");

            Assert.AreEqual(null, value1);
            Assert.AreEqual("1002", value2);
            Assert.AreEqual("1003", value3);
            Assert.GreaterOrEqual(_evictionEventOccuredCount,1);
        }

        private void Cache_EvictionOccured()
        {
            _evictionEventOccuredCount += 1;
        }
    }
}
