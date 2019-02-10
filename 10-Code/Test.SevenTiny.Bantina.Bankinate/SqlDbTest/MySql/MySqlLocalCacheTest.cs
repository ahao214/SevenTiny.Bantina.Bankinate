using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using Test.SevenTiny.Bantina.Bankinate.Helpers;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate.SqlDbTest.MySql
{
    [DataBase("SevenTinyTest")]
    public class MySqlLocalQueryCache : MySqlDbContext<MySqlDb>
    {
        public MySqlLocalQueryCache() : base(ConnectionStrings.Get("mysql39901"))
        {
            OpenQueryCache = true;//一级缓存开关
        }
    }
    [DataBase("SevenTinyTest")]
    public class MySqlLocalTableCache : MySqlDbContext<MySqlDb>
    {
        public MySqlLocalTableCache() : base(ConnectionStrings.Get("mysql39901"))
        {
            OpenTableCache = true;//二级缓存开关，表实体上的二级标签也需要提供
        }
    }

    public class MySqlLocalCacheTest : SqlDbCacheTest<MySqlDb>
    {
        public override SqlDbContext<MySqlDb> QueryCacheDb { get => new MySqlLocalQueryCache(); }
        public override SqlDbContext<MySqlDb> TableCacheDb { get => new MySqlLocalTableCache(); }

        [Fact]
        [Trait("bug", "两次查出来的结果不正确【由于内存做的缓存，改内存数据时缓存会一起变动...作为缓存时，慎改内存数据】")]
        [Trait("bug", "参数传递值没有使用参数化查询")]
        public void Query_BugRepaire2()
        {
            int metaObjectId = 1;
            using (var db = new MySqlLocalQueryCache())
            {
                for (int i = 0; i < 3; i++)
                {
                    var re = db.QueryList<OperateTestModel>(t => t.IntNullKey == 1 && t.IntKey == metaObjectId);
                    Assert.NotNull(re);
                }
            }
        }
    }
}
