using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using SevenTiny.Bantina.Bankinate.Helpers.Redis;
using System;
using Newtonsoft.Json;

namespace SevenTiny.Bantina.Bankinate.Cache
{
    /// <summary>
    /// 缓存存储媒介
    /// </summary>
    public enum CacheMediaType
    {
        /// <summary>
        /// 本地缓存
        /// </summary>
        Local = 0,
        /// <summary>
        /// Redis缓存
        /// </summary>
        Redis = 1
    }

    /// <summary>
    /// 缓存存储管理器
    /// </summary>
    internal abstract class CacheStorageManager
    {
        private static IRedisCache redisCache = null;
        private static IRedisCache GetRedisCacheProvider(DbContext dbContext)
        {
            if (redisCache != null)
                return redisCache;

            //配置的异常是参数异常，在处理数据时应抛出异常，其他连接异常应该忽略返回获取缓存失败
            if (string.IsNullOrEmpty(dbContext.CacheMediaServer))
                throw new ArgumentException("Cache server address error", "dbContext.CacheMediaServer");

            redisCache = new RedisCacheManager(dbContext.CacheMediaServer);

            if (redisCache == null)
                throw new Exception("redis init timeout");

            return redisCache;
        }

        public static bool IsExist(DbContext dbContext, string key)
        {
            return IsExist(dbContext, key, out object obj);
        }
        public static bool IsExist<TValue>(DbContext dbContext, string key, out TValue value)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Exist(key, out value);
                case CacheMediaType.Redis:
                    try
                    {
                        var redisResult = GetRedisCacheProvider(dbContext).Get(key);
                        if (!string.IsNullOrEmpty(redisResult))
                        {
                            value = JsonConvert.DeserializeObject<TValue>(redisResult);
                            return true;
                        }
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    value = default(TValue);
                    return false;
                default:
                    value = default(TValue);
                    return false;

            }
        }
        public static void Put<T>(DbContext dbContext, string key, T value, TimeSpan expiredTime)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Put(key, value, expiredTime);
                    break;
                case CacheMediaType.Redis:
                    try
                    {
                        GetRedisCacheProvider(dbContext).Set(key, JsonConvert.SerializeObject(value), expiredTime);
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    break;
                default:
                    break;
            }
        }
        public static T Get<T>(DbContext dbContext, string key)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    return MemoryCacheHelper.Get<string, T>(key);
                case CacheMediaType.Redis:
                    try
                    {
                        var redisResult = GetRedisCacheProvider(dbContext).Get(key);
                        if (!string.IsNullOrEmpty(redisResult))
                        {
                            return JsonConvert.DeserializeObject<T>(redisResult);
                        }
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    return default(T);
                default:
                    return default(T);
            }
        }
        public static void Delete(DbContext dbContext, string key)
        {
            switch (dbContext.CacheMediaType)
            {
                case CacheMediaType.Local:
                    MemoryCacheHelper.Delete<string>(key);
                    break;
                case CacheMediaType.Redis:
                    try
                    {
                        GetRedisCacheProvider(dbContext).Delete(key);
                    }
                    catch (ArgumentException argEx)
                    {
                        throw argEx;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
