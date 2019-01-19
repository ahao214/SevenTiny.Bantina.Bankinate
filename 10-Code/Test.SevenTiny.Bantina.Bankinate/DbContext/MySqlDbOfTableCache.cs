using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    [DataBase("SevenTinyTest")]
    public class MySqlDbOfTableCache : MySqlDbContext<MySqlDb>
    {
        public MySqlDbOfTableCache() : base(ConnectionStrings.Get("mysql39901"))
        {
            OpenTableCache = true;//二级缓存开关，表实体上的二级标签也需要提供
        }
    }
}
