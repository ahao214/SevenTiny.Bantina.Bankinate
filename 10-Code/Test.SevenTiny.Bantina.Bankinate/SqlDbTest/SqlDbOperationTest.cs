using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Linq;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 关系型数据库查询测试
    /// </summary>
    public abstract class SqlDbOperationTest<DataBase> where DataBase : class
    {
        //这里使用继承关系测试不同的关系型数据库操作
        public abstract SqlDbContext<DataBase> Db { get; }

        [Fact]
        [Trait("desc", "持久化测试")]
        public void Persistence()
        {
            int value = 999999;

            //初次查询没有数据
            var re = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
            Assert.Null(re);

            //add一条数据
            OperateTestModel model = new OperateTestModel
            {
                IntKey = value,
                StringKey = "AddTest"
            };
            model.IntKey = value;
            Db.Add<OperateTestModel>(model);

            //插入后查询有一条记录
            var re1 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
            Assert.Single(re1);
            Assert.Equal(value, re1.First().IntKey);

            //查询一条
            var entity = Db.QueryOne<OperateTestModel>(t => t.IntKey == value);
            Assert.NotNull(entity);
            Assert.Equal(value, entity.IntKey);

            //更新数据
            //entity.Id = value;   //自增的主键不应该被修改,如果用这种方式进行修改，给Id赋值就会导致修改不成功，因为条件是用第一个主键作为标识修改的
            entity.Key2 = value;
            entity.StringKey = $"UpdateTest_{value}";
            entity.IntNullKey = value;
            entity.DateTimeNullKey = DateTime.Now;
            entity.DateNullKey = DateTime.Now.Date;
            entity.DoubleNullKey = entity.IntNullKey;
            entity.FloatNullKey = entity.IntNullKey;
            Db.Update<OperateTestModel>(entity);

            var entity2 = Db.QueryOne<OperateTestModel>(t => t.IntKey == value);
            Assert.NotNull(entity2);
            Assert.Equal(value, entity2.IntNullKey);
            Assert.Equal($"UpdateTest_{value}", entity2.StringKey);

            //删除数据
            Db.Delete<OperateTestModel>(t => t.IntKey == value);

            //删除后查询没有
            var re4 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
            Assert.Null(re4);
        }

        [Fact]
        [Trait("desc", "持久化测试_默认使用实体主键删除数据")]
        public void Persistence_DeleteEntity()
        {
            int value = 999999;

            //初次查询没有数据
            var re = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
            Assert.Null(re);

            //add一条数据
            OperateTestModel model = new OperateTestModel
            {
                IntKey = value,
                StringKey = "AddTest"
            };
            model.IntKey = value;
            Db.Add<OperateTestModel>(model);

            //插入后查询有一条记录
            var re1 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
            Assert.Single(re1);
            Assert.Equal(value, re1.First().IntKey);

            var entity = Db.QueryOne<OperateTestModel>(t => t.IntKey == value);
            Assert.NotNull(entity);
            Assert.Equal(value, entity.IntKey);

            //删除数据
            Db.Delete<OperateTestModel>(entity);

            //删除后查询没有
            var re4 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("AddTest")).ToList();
            Assert.Null(re4);
        }

        [Fact]
        public void Query_All()
        {
            var re = Db.Queryable<OperateTestModel>().ToList();
            Assert.Equal(1000, re.Count);
        }

        [Fact]
        public void Query_Where()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.EndsWith("3")).ToList();
            Assert.Equal(100, re.Count);
        }

        [Fact]
        public void Query_MultiWhere()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("3")).Where(t => t.IntKey == 3).ToList();
            Assert.Single(re);
        }

        [Fact]
        public void Query_Select()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.IntKey <= 3).Select(t => new { t.IntKey, t.StringKey }).ToList();
            Assert.Equal(3, re.Count);
        }

        [Fact]
        public void Query_OrderBy()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.IntKey <= 9).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).ToList();
            Assert.True(re.Count == 9 && re.First().IntKey == 9 && re.First().Id == 0);//没有查id，id应该=0
        }

        [Fact]
        public void Query_Limit()
        {
            var re = Db.Queryable<OperateTestModel>().Where(t => t.IntKey > 3).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Limit(30).ToList();
            Assert.Equal(30, re.Count);
        }

        [Fact]
        public void Query_Paging()
        {
            var re4 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(0, 10).ToList();
            var re5 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderByDescending(t => t.IntKey).Paging(0, 10).ToList();
            var re6 = Db.Queryable<OperateTestModel>().Where(t => t.StringKey.Contains("1")).Select(t => new { t.IntKey, t.StringKey }).OrderBy(t => t.IntKey).Paging(1, 10).ToList();
            Assert.True(re4.Count == re5.Count && re5.Count == re6.Count && re6.Count == re4.Count);
        }

        [Fact]
        public void Query_Any()
        {
            var re = Db.Queryable<OperateTestModel>().Any(t => t.StringKey.EndsWith("3"));
            Assert.True(re);
        }

        [Fact]
        [Trait("bug", "修复同字段不同值的，sql和参数生成错误")]
        [Trait("bug", "修复生成sql语句由于没有括号，逻辑顺序有误")]
        public void Query_BugRepaire1()
        {
            var re = Db.QueryOne<OperateTestModel>(t => t.IntKey == 1 && t.Id != 2 && (t.StringKey.Contains("1") || t.StringKey.Contains("2")));
            Assert.NotNull(re);
        }
    }
}
