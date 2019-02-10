using SevenTiny.Bantina.Bankinate;
using bankinate = SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using Test.SevenTiny.Bantina.Bankinate.Helpers;

namespace Test.SevenTiny.Bantina.Bankinate.SqlDbTest.MySql
{
    [DataBase("SevenTinyTest")]
    public class MySqlRedisQueryCache : MySqlDbContext<MySqlDb>
    {
        public MySqlRedisQueryCache() : base(ConnectionStrings.Get("mysql39901"))
        {
            OpenQueryCache = true;//一级缓存开关
            CacheMediaType = bankinate.Cache.CacheMediaType.Redis;
            CacheMediaServer = "192.168.1.110:39912";//redis服务器地址以及端口号
        }
    }
    [DataBase("SevenTinyTest")]
    public class MySqlRedisTableCache : MySqlDbContext<MySqlDb>
    {
        public MySqlRedisTableCache() : base(ConnectionStrings.Get("mysql39901"))
        {
            OpenTableCache = true;//二级缓存开关，表实体上的二级标签也需要提供
            CacheMediaType = bankinate.Cache.CacheMediaType.Redis;
            CacheMediaServer = "192.168.1.110:39912";//redis服务器地址以及端口号
        }
    }

    public class MySqlRedisCacheTest : SqlDbCacheTest<MySqlDb>
    {
        public override SqlDbContext<MySqlDb> QueryCacheDb { get => new MySqlRedisQueryCache(); }
        public override SqlDbContext<MySqlDb> TableCacheDb { get => new MySqlRedisTableCache(); }
    }
}
