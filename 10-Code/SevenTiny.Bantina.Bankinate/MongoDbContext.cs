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
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MongoDbContext<TDataBase> : NoSqlDbContext<TDataBase> where TDataBase : class
    {
        protected MongoDbContext(string connectionString) : base(DataBaseType.MongoDB, connectionString) { }
        protected MongoDbContext(MongoClientSettings mongoClientSettings) : base(mongoClientSettings) { }

        public IMongoCollection<BsonDocument> BsonCollection => DataBase.GetCollection<BsonDocument>(typeof(BsonDocument).Name);
        public IMongoQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return GetCollectionEntity<TEntity>().AsQueryable();
        }

        public void Add(BsonDocument bsonDocument)
        {
            BsonCollection.InsertOne(bsonDocument);
        }
        public void AddAsync(BsonDocument bsonDocument)
        {
            BsonCollection.InsertOneAsync(bsonDocument);
        }
        public void Add(IEnumerable<BsonDocument> bsonDocuments)
        {
            BsonCollection.InsertMany(bsonDocuments);
        }
        public void AddAsync(IEnumerable<BsonDocument> bsonDocuments)
        {
            BsonCollection.InsertManyAsync(bsonDocuments);
        }

        public void Update(FilterDefinition<BsonDocument> filter, BsonDocument replacement)
        {
            BsonCollection.ReplaceOne(filter, replacement);
        }
        public void UpdateAsync(FilterDefinition<BsonDocument> filter, BsonDocument replacement)
        {
            BsonCollection.ReplaceOneAsync(filter, replacement);
        }

        public void DeleteOne(FilterDefinition<BsonDocument> filter)
        {
            BsonCollection.DeleteOne(filter);
        }
        public void Delete(FilterDefinition<BsonDocument> filter)
        {
            BsonCollection.DeleteMany(filter);
        }
        public void DeleteAsync(FilterDefinition<BsonDocument> filter)
        {
            BsonCollection.DeleteManyAsync(filter);
        }

        public BsonDocument QueryOneBson(string _id)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", _id);
            return BsonCollection.Find(filter).FirstOrDefault();
        }
        public BsonDocument QueryOneBson(FilterDefinition<BsonDocument> filter)
        {
            return BsonCollection.Find(filter).FirstOrDefault();
        }

        public List<BsonDocument> QueryListBson(FilterDefinition<BsonDocument> filter)
        {
            return BsonCollection.Find(filter).ToList();
        }
        public List<BsonDocument> QueryListBson(FilterDefinition<BsonDocument> filter, int pageIndex, int pageSize)
        {
            return BsonCollection.Find(filter).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
        }

        public int QueryCount(FilterDefinition<BsonDocument> filter)
        {
            return Convert.ToInt32(BsonCollection.Count(filter));
        }
        public bool QueryExist(FilterDefinition<BsonDocument> filter)
        {
            return QueryCount(filter) > 0;
        }
    }
}
