using System;
using System.Runtime.Caching;

namespace CSharpHelper
{
    public static class MemoryCacheHelper
    {
        private static readonly object _locker = new object();

        public static T GetCache<T>(string key, Func<T> cachePopulate, TimeSpan? slidingExpiration = null, DateTime? absoluteExpiration = null) where T : class
        {
            if (MemoryCache.Default[key] == null)
            {
                lock (MemoryCacheHelper._locker)
                {
                    if (MemoryCache.Default[key] == null)
                    {
                        CacheItem item = new CacheItem(key, cachePopulate());
                        CacheItemPolicy policy = MemoryCacheHelper.CreatePolicy(slidingExpiration, absoluteExpiration);
                        MemoryCache.Default.Add(item, policy);
                    }
                }
            }
            return MemoryCache.Default[key] as T;
        }

        private static CacheItemPolicy CreatePolicy(TimeSpan? slidingExpiration, DateTime? absoluteExpiration)
        {
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            if (slidingExpiration.HasValue)
            {
                cacheItemPolicy.SlidingExpiration = slidingExpiration.Value;
            }
            if (absoluteExpiration.HasValue)
            {
                cacheItemPolicy.AbsoluteExpiration = absoluteExpiration.Value;
            }
            cacheItemPolicy.Priority = CacheItemPriority.Default;
            return cacheItemPolicy;
        }
    }
}
