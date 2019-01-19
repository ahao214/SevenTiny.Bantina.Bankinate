using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    [DataBase("SevenTinyTest")]
    public class MySqlDbOfQueryCache : MySqlDbContext<MySqlDb>
    {
        public MySqlDbOfQueryCache() : base(ConnectionStrings.Get("mysql39901"))
        {
            OpenQueryCache = true;//一级缓存开关
        }
    }
}
