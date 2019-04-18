using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.CacheManagement
{
    /// <summary>
    /// 查询缓存管理器（一级缓存管理器）
    ///QueryCache存储结构，以表为缓存单位，便于在对单个表进行操作以后释放单个表的缓存，每个表的缓存以hash字典的方式存储
    ///Key(表相关Key)
    ///Dictionary<int, T>
    ///{
    ///     sql.HashCode(),值
    ///}
    /// </summary>
    internal abstract class QueryCacheManager
    {
        /// <summary>
        /// 清空所有缓存
        /// </summary>
        internal static void FlushAllCache(SqlDbContext dbContext)
        {
            if (CacheStorageManager.IsExist(dbContext, BankinateConst.GetQueryCacheKeysCacheKey(dbContext.DataBaseName), out HashSet<string> keys))
            {
                foreach (var item in keys)
                {
                    CacheStorageManager.Delete(dbContext, item);
                }
            }
        }

        /// <summary>
        /// 清空单个表相关的所有缓存
        /// </summary>
        /// <param name="dbContext"></param>
        internal static void FlushTableCache(SqlDbContext dbContext)
        {
            CacheStorageManager.Delete(dbContext, GetQueryCacheKey(dbContext));
        }

        /// <summary>
        /// 构建sql查询缓存的总key
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private static string GetQueryCacheKey(SqlDbContext dbContext)
        {
            string key = $"{BankinateConst.CacheKey_QueryCache}{dbContext.TableName}";
            //缓存键更新
            if (!CacheStorageManager.IsExist(dbContext, BankinateConst.GetQueryCacheKeysCacheKey(dbContext.DataBaseName), out HashSet<string> keys))
            {
                keys = new HashSet<string>();
            }
            keys.Add(key);
            CacheStorageManager.Put(dbContext, BankinateConst.GetQueryCacheKeysCacheKey(dbContext.DataBaseName), keys, dbContext.MaxExpiredTimeSpan);
            return key;
        }

        /// <summary>
        /// 构建sql查询的sql语句缓存键Key
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private static int GetSqlQueryCacheKey(SqlDbContext dbContext)
        {
            //如果有条件，则sql的key要拼接对应的参数值
            if (dbContext.Parameters != null && dbContext.Parameters.Any())
            {
                return $"{dbContext.SqlStatement}_{string.Join("|", dbContext.Parameters.Values)}".GetHashCode();
            }
            return dbContext.SqlStatement.GetHashCode();
        }

        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        internal static T GetEntitiesFromCache<T>(SqlDbContext dbContext)
        {
            //1.检查是否开启了Query缓存
            if (dbContext.OpenQueryCache)
            {
                //2.如果QueryCache里面有该缓存键，则直接获取，并从单个表单位中获取到对应sql的值
                if (CacheStorageManager.IsExist(dbContext, GetQueryCacheKey(dbContext), out Dictionary<int, object> t))
                {
                    int sqlQueryCacheKey = GetSqlQueryCacheKey(dbContext);
                    if (t.ContainsKey(sqlQueryCacheKey))
                    {
                        dbContext.IsFromCache = true;
                        return TypeConvertHelper.ToGenericType<T>(t[sqlQueryCacheKey]);
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// QueryCache级别存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="cacheValue"></param>
        internal static void CacheData<T>(SqlDbContext dbContext, T cacheValue)
        {
            if (dbContext.OpenQueryCache)
            {
                if (cacheValue != null)
                {
                    int sqlQueryCacheKey = GetSqlQueryCacheKey(dbContext);
                    //如果缓存中存在，则拿到表单位的缓存并更新
                    //这里用object类型进行存储，因为字典的value可能有list集合，int，object等多种类型，泛型使用会出现识别异常
                    if (CacheStorageManager.IsExist(dbContext, GetQueryCacheKey(dbContext), out Dictionary<int, object> t))
                    {
                        //如果超出单表的query缓存键阈值，则按先后顺序进行移除
                        if (t.Count >= dbContext.QueryCacheMaxCountPerTable)
                            t.Remove(t.First().Key);

                        t.AddOrUpdate(sqlQueryCacheKey, cacheValue);
                        CacheStorageManager.Put(dbContext, GetQueryCacheKey(dbContext), t, dbContext.QueryCacheExpiredTimeSpan);
                    }
                    //如果缓存中没有表单位的缓存，则直接新增表单位的sql键缓存
                    else
                    {
                        var dic = new Dictionary<int, object> { { sqlQueryCacheKey, cacheValue } };
                        CacheStorageManager.Put(dbContext, GetQueryCacheKey(dbContext), dic, dbContext.QueryCacheExpiredTimeSpan);
                    }
                }
            }
        }
    }
}
