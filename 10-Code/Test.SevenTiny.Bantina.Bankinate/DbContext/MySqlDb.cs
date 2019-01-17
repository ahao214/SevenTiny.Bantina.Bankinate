using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    [DataBase("SevenTinyTest")]
    public class MySqlDb : MySqlDbContext<MySqlDb>
    {
        public MySqlDb() : base(ConnectionStrings.Get("mysql39901"))
        {
            //OpenTableCache = true;//二级缓存开关，表实体上的二级标签也需要提供
            //OpenQueryCache = true;//一级缓存开关
        }
    }

    [DataBase("SevenTinyTest")]
    public class MySqlPropertyValidateDb : MySqlDbContext<MySqlPropertyValidateDb>
    {
        public MySqlPropertyValidateDb() : base(ConnectionStrings.Get("mysql39901"))
        {
            OpenPropertyDataValidate = true;
            OpenRealExecutionSaveToDb = false;
        }
    }
}
