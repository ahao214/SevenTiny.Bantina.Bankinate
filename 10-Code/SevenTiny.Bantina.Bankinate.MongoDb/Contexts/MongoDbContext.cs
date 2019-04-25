/*********************************************************
 * CopyRight: 7TINY CODE BUILDER. 
 * Version: 5.0.0
 * Author: 7tiny
 * Address: Earth
 * Create: 2018-04-19 23:58:08
 * Modify: 2018-04-19 23:58:08
 * E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
 * GitHub: https://github.com/sevenTiny 
 * Personal web site: http://www.7tiny.com 
 * Technical WebSit: http://www.cnblogs.com/7tiny/ 
 * Description: 
 * Thx , Best Regards ~
 *********************************************************/
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MongoDbContext<TDataBase> : NoSqlDbContext where TDataBase : class
    {
        protected MongoDbContext(string connectionString) : base(connectionString)
        {
            SetContext();
            Client = new MongoClient(connectionString);
        }
        protected MongoDbContext(string host, int port) : base(string.Concat(host, ":", port))
        {
            SetContext();
            Client = new MongoClient(new MongoClientSettings { Server = new MongoServerAddress(host, port) });
        }
        protected MongoDbContext(IDictionary<string, int> host_port_dic) : base("123")    //mongodb 不用连接管理器托管字符串管理，所以这里随便能传递了一个连接字符串
        {
            SetContext();

            if (host_port_dic == null || !host_port_dic.Any())
                throw new ArgumentException($"argument of '{nameof(host_port_dic)}' can not be null!");

            Client = new MongoClient(new MongoClientSettings
            {
                Servers = host_port_dic.Select(t => new MongoServerAddress(t.Key, t.Value)).ToList()
            });
        }
        protected MongoDbContext(MongoClientSettings mongoClientSettings) : base("123")    //mongodb 不用连接管理器托管字符串管理，所以这里随便能传递了一个连接字符串
        {
            SetContext();
            Client = new MongoClient(mongoClientSettings);
        }
        /// <summary>
        /// 上下文赋值
        /// </summary>
        private void SetContext()
        {
            DataBaseType = DataBaseType.MongoDB;
            DataBaseName = DataBaseAttribute.GetName(typeof(TDataBase));
        }

        #region MongoDb Server
        private MongoClient Client { get; set; }
        private IMongoDatabase DataBase => Client.GetDatabase(DataBaseName);
        private IMongoCollection<TEntity> GetCollectionEntity<TEntity>() where TEntity : class
        {
            CollectionName = TableAttribute.GetName(typeof(TEntity));
            return DataBase.GetCollection<TEntity>(CollectionName);
        }
        /// <summary>
        /// 对外支持弱类型的接口
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> GetCollectionBson(string collectionName)
        {
            CollectionName = collectionName;
            return DataBase.GetCollection<BsonDocument>(collectionName);
        }
        #endregion

        #region 强类型 API
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            GetCollectionEntity<TEntity>().InsertOne(entity);
            DbCacheManager.Add(entity);
        }
        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            await GetCollectionEntity<TEntity>().InsertOneAsync(entity);
            DbCacheManager.Add(entity);
        }
        public void Add<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entities);
            GetCollectionEntity<TEntity>().InsertMany(entities);
            DbCacheManager.Add(entities);
        }
        public async Task AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entities);
            await GetCollectionEntity<TEntity>().InsertManyAsync(entities);
            DbCacheManager.Add(entities);
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            QueryCacheKey = filter.ToString();
            GetCollectionEntity<TEntity>().ReplaceOne(filter, entity);
            DbCacheManager.Update(entity, filter);
        }
        public async Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            QueryCacheKey = filter.ToString();
            await GetCollectionEntity<TEntity>().ReplaceOneAsync(filter, entity);
            DbCacheManager.Update(entity, filter);
        }

        public void DeleteOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            GetCollectionEntity<TEntity>().DeleteOne(filter);
            DbCacheManager.Delete(filter);
        }
        public async Task DeleteOneAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            await GetCollectionEntity<TEntity>().DeleteOneAsync(filter);
            DbCacheManager.Delete(filter);
        }
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            QueryCacheKey = filter.ToString();
            GetCollectionEntity<TEntity>().DeleteMany(filter);
            DbCacheManager.Delete(filter);
        }
        public async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            QueryCacheKey = filter.ToString();
            await GetCollectionEntity<TEntity>().DeleteManyAsync(filter);
            DbCacheManager.Delete(filter);
        }

        public long QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            QueryCacheKey = filter.ToString();
            return DbCacheManager.GetCount(filter, () =>
            {
                return GetCollectionEntity<TEntity>().CountDocuments(filter);
            });
        }
        public bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return QueryCount<TEntity>(filter) > 0;
        }
        public TEntity QueryOne<TEntity>(string _id) where TEntity : class
        {
            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", _id);
            return GetCollectionEntity<TEntity>().Find(filter).FirstOrDefault();
        }
        public TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            QueryCacheKey = filter.ToString();
            return DbCacheManager.GetEntity(filter, () =>
            {
                return QueryList(filter).FirstOrDefault();
            });
        }
        public List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            QueryCacheKey = filter.ToString();
            return DbCacheManager.GetEntities(filter, () =>
            {
                return GetCollectionEntity<TEntity>().Find(filter).ToList();
            });
        }

        public IMongoQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return GetCollectionEntity<TEntity>().AsQueryable();
        }
        #endregion

        /// <summary>
        /// 获取全集合数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        internal override List<TEntity> GetFullCollectionData<TEntity>()
        {
            //获取MongoDb全文档记录
            return GetCollectionEntity<TEntity>().Find(t => true).ToList();
        }
    }
}
