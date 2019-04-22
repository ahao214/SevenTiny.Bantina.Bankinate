using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate.CacheManagement
{
    /// <summary>
    /// 数据库缓存管理器
    /// Bankinate的缓存是分为两级的，每级都有对应的开关
    /// 一级缓存（QueryCache查询缓存），缓存简短查询中的缓存数据
    /// 二级缓存（TableCache表缓存），缓存整张表，需要标签配合使用
    /// </summary>
    internal class DbCacheManager : CacheManagerBase
    {
        public DbCacheManager(SqlDbContext context) : base(context)
        {
            if (context.OpenQueryCache)
                QueryCacheManager = new QueryCacheManager(context);
            if (context.OpenTableCache)
                TableCacheManager = new TableCacheManager(context);
        }

        public QueryCacheManager QueryCacheManager { get; set; }
        public TableCacheManager TableCacheManager { get; set; }

        /// 清空所有缓存
        /// </summary>
        internal void FlushAllCache()
        {
            QueryCacheManager.FlushAllCache();
            TableCacheManager.FlushAllCache();
        }
        /// <summary>
        /// 清空单个表相关的所有缓存
        /// </summary>
        internal void FlushCurrentTableCache()
        {
            QueryCacheManager.FlushTableCache();
            TableCacheManager.FlushTableCache();
        }

        internal void Add<TEntity>(TEntity entity)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache();
            //2.更新Table缓存中的该表记录
            TableCacheManager.AddCache(entity);
        }
        internal void Add<TEntity>(IEnumerable<TEntity> entities)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache();
            //2.更新Table缓存中的该表记录
            TableCacheManager.AddCache(entities);
        }
        internal void Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache();
            //2.更新Table缓存中的该表记录
            TableCacheManager.UpdateCache(entity, filter);
        }
        internal void Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache();
            //2.更新Table缓存中的该表记录
            TableCacheManager.DeleteCache(filter);
        }
        internal void Delete<TEntity>(TEntity entity)
        {
            //1.清空Query缓存中关于该表的所有缓存记录
            QueryCacheManager.FlushTableCache();
            //2.更新Table缓存中的该表记录
            TableCacheManager.DeleteCache(entity);
        }

        internal List<TEntity> GetEntities<TEntity>(Expression<Func<TEntity, bool>> filter, Func<List<TEntity>> func) where TEntity : class
        {
            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            var entities = TableCacheManager.GetEntitiesFromCache(filter);

            //2.判断是否在一级QueryCahe中
            if (entities == null || !entities.Any())
            {
                entities = QueryCacheManager.GetEntitiesFromCache<List<TEntity>>();
            }

            //3.如果都没有，则直接从逻辑中获取
            if (entities == null || !entities.Any())
            {
                entities = func();
                DbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(entities);
            }

            return entities;
        }
        internal TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> filter, Func<TEntity> func) where TEntity : class
        {
            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            var result = TableCacheManager.GetEntitiesFromCache(filter)?.FirstOrDefault();

            //2.判断是否在一级QueryCahe中
            if (result == null)
            {
                result = QueryCacheManager.GetEntitiesFromCache<TEntity>();
            }

            //3.如果都没有，则直接从逻辑中获取
            if (result == null || result == default(TEntity))
            {
                result = func();
                DbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(result);
            }

            return result;
        }
        internal int GetCount<TEntity>(Expression<Func<TEntity, bool>> filter, Func<int> func) where TEntity : class
        {
            //1.判断是否在二级TableCache，如果没有，则进行二级缓存初始化逻辑
            var result = TableCacheManager.GetEntitiesFromCache(filter)?.Count;

            //2.判断是否在一级QueryCahe中
            if (result == null)
            {
                result = QueryCacheManager.GetEntitiesFromCache<int?>();
            }

            //3.如果都没有，则直接从逻辑中获取
            if (result == null || result == default(int?))
            {
                result = func();
                DbContext.IsFromCache = false;
                //4.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(result);
            }

            return result ?? default(int);
        }
        internal T GetObject<T>(Func<T> func) where T : class
        {
            //1.判断是否在一级QueryCahe中
            var result = QueryCacheManager.GetEntitiesFromCache<T>();

            //2.如果都没有，则直接从逻辑中获取
            if (result == null)
            {
                result = func();
                DbContext.IsFromCache = false;
                //3.Query缓存存储逻辑（内涵缓存开启校验）
                QueryCacheManager.CacheData(result);
            }

            return result;
        }
    }
}
