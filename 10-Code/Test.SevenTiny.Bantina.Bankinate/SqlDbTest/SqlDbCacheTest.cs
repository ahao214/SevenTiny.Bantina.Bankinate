using SevenTiny.Bantina;
using SevenTiny.Bantina.Bankinate.DbContexts;
using System.Diagnostics;
using System.Linq;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 关系型数据库缓存测试
    /// </summary>
    public abstract class SqlDbCacheTest<DataBase> where DataBase : class
    {
        //这里切换对应的关系型数据库上下文
        public abstract SqlDbContext<DataBase> QueryCacheDb { get; }
        public abstract SqlDbContext<DataBase> TableCacheDb { get; }

        [Fact]
        public void QueryCache_Query_Add()
        {
            //1.先查询肯定是没有的
            var re0 = QueryCacheDb.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
            Assert.Null(re0);

            QueryCacheDb.Add(new OperateTestModel
            {
                IntKey = 123,
                StringKey = "CacheAddTest123"
            });

            //2.这时候查询应该有一条，这次查询才加入缓存
            var re = QueryCacheDb.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
            Assert.Single(re);

            //3.重复查询，这次是从缓存查的，还是一条
            var re2 = QueryCacheDb.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
            Assert.Single(re2);

            //再次新增，清楚一级缓存
            QueryCacheDb.Add(new OperateTestModel
            {
                IntKey = 123,
                StringKey = "CacheAddTest123"
            });

            //4.这次查应该从数据库查询，加入缓存，2条
            var re4 = QueryCacheDb.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
            Assert.Equal(2, re4.Count);

            QueryCacheDb.Delete<OperateTestModel>(t => t.StringKey.StartsWith("CacheAddTest"));

            //4.删除完毕以后，查询是没有的
            var re3 = QueryCacheDb.Queryable<OperateTestModel>().Where(t => t.StringKey.StartsWith("CacheAddTest")).ToList();
            Assert.Null(re3);
        }

        [Theory]
        [InlineData(100)]
        public void QueryCache_Query_All(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var re = QueryCacheDb.Queryable<OperateTestModel>().ToList();
                Assert.Equal(1000, re.Count);
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryCache_Query_One(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var re = QueryCacheDb.QueryOne<OperateTestModel>(t => t.StringKey.Contains("test"));
                Assert.NotNull(re);
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryCache_Query_Count(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var re = QueryCacheDb.QueryCount<OperateTestModel>(t => t.StringKey.Contains("test"));
                Assert.Equal(1000, re);
            }
        }

        [Theory]
        [InlineData(100)]
        public void QueryCache_Query_WhereWithUnSameCondition(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var re = QueryCacheDb.QueryOne<OperateTestModel>(t => t.Id == 1);
                var re1 = QueryCacheDb.QueryOne<OperateTestModel>(t => t.Id == 2);
                Assert.NotEqual(re.StringKey, re1.StringKey);
            }
        }

        [Theory]
        [InlineData(100)]
        [Trait("desc", "设置缓存过期时间进行测试")]
        public void QueryCache_Expired(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var re = QueryCacheDb.QueryOne<OperateTestModel>(t => t.StringKey.Contains("test"));
                Assert.NotNull(re);
            }
        }

        //下面方法需要重构 -------------------------------

        //[Theory]
        //[InlineData(100)]
        //[Trait("desc", "无缓存测试")]
        //public void PerformanceTest_QueryListWithNoCacheLevel1(int times)
        //{
        //    return;

        //    int fromCacheTimes = 0;
        //    var timeSpan = StopwatchHelper.Caculate(times, () =>
        //    {
        //        using (var db = new SqlServerDb())
        //        {
        //            var students = db.Queryable<Student>().Where(t => true).ToList();
        //            if (db.IsFromCache)
        //            {
        //                fromCacheTimes++;
        //            }
        //        }
        //    });
        //    Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
        //    //执行查询100次耗时：6576.8009
        //}

        //[Theory]
        //[InlineData(10000)]
        //[Trait("desc", "一级缓存测试")]
        //[Trait("desc", "测试该用例，请将一级缓存（QueryCache）打开")]
        //public void PerformanceTest_QueryListWithCacheLevel1(int times)
        //{
        //    return;

        //    int fromCacheTimes = 0;
        //    var timeSpan = StopwatchHelper.Caculate(times, () =>
        //    {
        //        using (var db = new SqlServerDb())
        //        {
        //            var students = db.Queryable<Student>().Where(t => true).ToList();
        //            if (db.IsFromCache)
        //            {
        //                fromCacheTimes++;
        //            }
        //        }
        //    });
        //    Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
        //    //执行查询10000次耗时：1598.2349
        //}

        //[Theory]
        //[InlineData(10000)]
        //[Trait("desc", "二级缓存测试")]
        //[Trait("desc", "测试该用例，请将二级缓存（TableCache）打开，并在对应表的实体上添加缓存标签")]
        //public void PerformanceTest_QueryListWithCacheLevel2(int times)
        //{
        //    return;

        //    int fromCacheTimes = 0;
        //    var timeSpan = StopwatchHelper.Caculate(times, () =>
        //    {
        //        using (var db = new SqlServerDb())
        //        {
        //            var students = db.Queryable<Student>().Where(t => true).ToList();
        //            if (db.IsFromCache)
        //            {
        //                fromCacheTimes++;
        //            }
        //        }
        //    });
        //    Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
        //    //执行查询10000次耗时：5846.0249，有9999次从缓存中获取，有1次从数据库获取。
        //    //通过更为详细的打点得知，共有两次从数据库获取值。第一次直接按条件查询存在一级缓存，后台线程扫描表存在了二级缓存。
        //    //缓存打点结果：二级缓存没有扫描完毕从一级缓存获取数据，二级缓存扫描完毕则都从二级缓存里面获取数据
        //}

        //[Theory]
        //[InlineData(1000)]
        //[Trait("desc", "开启二级缓存增删改查测试")]
        //[Trait("desc", "测试该用例，请将二级缓存（TableCache）打开，并在对应表的实体上添加缓存标签")]
        //public void PerformanceTest_AddUpdateDeleteQueryCacheLevel2(int times)
        //{
        //    return;

        //    int fromCacheTimes = 0;
        //    var timeSpan = StopwatchHelper.Caculate(times, () =>
        //    {
        //        using (var db = new SqlServerDb())
        //        {
        //            //查询单个
        //            var stu = db.QueryOne<Student>(t => t.Id == 2);
        //            //修改单个属性
        //            stu.Name = "test11-1";
        //            db.Update<Student>(t => t.Id == 1, stu);

        //            var students = db.Queryable<Student>().Where(t => true).ToList();
        //            if (db.IsFromCache)
        //            {
        //                fromCacheTimes++;
        //            }
        //        }
        //    });
        //    Trace.WriteLine($"执行查询{times}次耗时：{timeSpan.TotalMilliseconds}，有{fromCacheTimes}次从缓存中获取，有{times - fromCacheTimes}次从数据库获取");
        //    //执行查询1000次耗时：19102.6441，有1000次从缓存中获取，有0次从数据库获取
        //    //事实上，第一次查询单条的时候已经从数据库扫描并放在了缓存中，后续都是对二级缓存的操作以及二级缓存中查询
        //}
    }
}
