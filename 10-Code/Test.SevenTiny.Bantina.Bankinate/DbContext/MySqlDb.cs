using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    [DataBase("SevenTinyTest")]
    public class MySqlDb : MySqlDbContext<MySqlDb>
    {
        public MySqlDb() : base(ConnectionStrings.Get("mysql39901"))
        {
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
