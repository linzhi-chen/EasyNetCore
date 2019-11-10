﻿using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLF.Common.Caching
{
    public partial class MemoryCacheManager : ILocker, IStaticCacheManager
    {
        private readonly IEasyCachingProvider _provider;

        public MemoryCacheManager(IEasyCachingProvider provider)
        {
            _provider = provider;
        }

        #region Methods

        public T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
        {
            if (_provider.Exists(key))
            {
                return _provider.Get<T>(key).Value;
            }

            if ((cacheTime ?? CachingDefaultSettings.CacheTime) > 0)
            {
                _provider.Set(key, acquire, TimeSpan.FromMinutes(cacheTime ?? CachingDefaultSettings.CacheTime));
            }
            return acquire();
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
        {
            if (await _provider.ExistsAsync(key))
            {
                return (await _provider.GetAsync<T>(key)).Value;
            }

            if ((cacheTime ?? CachingDefaultSettings.CacheTime) > 0)
            {
                 await _provider.SetAsync(key, acquire, TimeSpan.FromMinutes(cacheTime ?? CachingDefaultSettings.CacheTime));
            }

            return await acquire();
        }

        public void Set(string key, object data, int cacheTime)
        {
            if (cacheTime <= 0)
                return;

            _provider.Set(key, data, TimeSpan.FromMinutes(cacheTime));
        }

        public bool IsSet(string key)
        {
            return _provider.Exists(key);
        }

        public bool PerformActionWithLock(string source, TimeSpan expirationTime, Action action)
        {
            //缓存中存在指定key
            if (_provider.Exists(source))
                return false;

            try
            {
                //开始锁定方法
                _provider.Set(source, source, expirationTime);
                action();

                return true;
            }
            finally
            {
                //释放锁
                Remove(source);
            }
        }

        public void Remove(string key)
        {
            _provider.Remove(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            _provider.RemoveByPrefix(prefix);
        }

        public void Clear()
        {
            _provider.Flush();
        }

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
