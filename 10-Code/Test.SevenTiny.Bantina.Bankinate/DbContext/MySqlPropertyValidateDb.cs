using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    /// <summary>
    /// 数据校验上下文
    /// </summary>
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
