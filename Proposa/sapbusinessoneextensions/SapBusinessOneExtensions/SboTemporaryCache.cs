using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace SapBusinessOneExtensions
{
    public class SboTemporaryCache : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static MemoryCache Cache { get; set; }
        private static SboTemporaryCache _instance;

        private SboTemporaryCache()
        {
            Cache = new MemoryCache("SboTemporaryLoopCache");
        }

        public static SboTemporaryCache Create()
        {
            if (_instance == null)
                _instance = new SboTemporaryCache();

            return _instance;
        }

        public static bool Contains(string cacheKey)
        {
            return Cache != null && Cache.Contains(cacheKey);
        }

        public static T Get<T>(string cacheKey)
        {
            if (!Contains(cacheKey))
                return (T) (object) null;

            Logger.Trace("Getting object from cache: {0}", cacheKey);
            return (T) Cache.Get(cacheKey);
        }

        public static T Set<T>(string cacheKey, T obj)
        {
            if (Cache == null || obj == null)
                return obj;

            Logger.Trace("Caching object: {0}", cacheKey);
            Cache.Set(cacheKey, obj, new DateTimeOffset(new DateTime(2199, 12, 31)));
            return obj;
        }

        public void Dispose()
        {
            Cache.Dispose();
            Cache = null;
            _instance = null;
        }
    }
}
