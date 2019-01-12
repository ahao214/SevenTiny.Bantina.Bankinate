using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using Test.SevenTiny.Bantina.Bankinate.DbContext;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    public class MongoDbTest
    {
        [Fact]
        public void GetConnectionString()
        {
            var conn = ConnectionStrings.Get("mongodb39911");
        }

        [Fact]
        public void Add()
        {
            List<BsonDocument> bsons = new List<BsonDocument>();
            for (int i = 0; i < 100; i++)
            {
                bsons.Add(new BsonDocument
                {
                    { "name", $"7tiny_{i}" },
                    { "age", i },
                    { "sex", new Random(DateTime.Now.Millisecond).Next(3) },
                });
            }

            using (MongoDb db = new MongoDb())
            {
                db.Add<Student>(bsons);
            }
        }

        [Fact]
        public void Delete()
        {
            using (var db = new MongoDb())
            {
                db.Delete<Student>(t => true);
            }
        }

        [Fact]
        public void Search_Eq()
        {
            using (var db = new MongoDb())
            {
                var filter = Builders<BsonDocument>.Filter.Eq("name", "7tiny_9");
                var result = db.QueryListBson<Student>(filter);
            }
        }

        /// <summary>
        /// �����ֶβ�ѯ
        /// </summary>
        [Fact]
        public void Search_AnyEq()
        {
            using (var db = new MongoDb())
            {
                //�����Ӳ���һ������
                var filter = Builders<BsonDocument>.Filter.AnyEq("name", "7tiny_9");
                var result = db.QueryListBson<Student>(filter);
            }
        }

        /// <summary>
        /// Less Than
        /// </summary>
        [Fact]
        public void Search_Lt()
        {
            using (var db = new MongoDb())
            {
                var filter = Builders<BsonDocument>.Filter.Lt("age", 10);
                var result = db.QueryListBson<Student>(filter);
            }
        }

        /// <summary>
        /// Greater Than
        /// </summary>
        [Fact]
        public void Search_Gt()
        {
            using (var db = new MongoDb())
            {
                var filter = Builders<BsonDocument>.Filter.Gt("age", 30);
                var result = db.QueryListBson<Student>(filter);
            }
        }
    }
}
