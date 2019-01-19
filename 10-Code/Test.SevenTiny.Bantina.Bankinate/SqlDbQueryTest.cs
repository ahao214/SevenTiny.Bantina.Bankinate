using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.DbContext;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 关系型数据库查询测试
    /// </summary>
    public class SqlDbQueryTest
    {
        //这里切换对应的关系型数据库上下文来测试不同的关系型数据库操作
        MySqlDb Db => new MySqlDb();
        //SqlServerDb Db => new SqlServerDb();

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
    }
}
