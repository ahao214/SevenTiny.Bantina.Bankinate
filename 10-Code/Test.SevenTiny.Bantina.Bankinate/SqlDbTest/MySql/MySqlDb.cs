using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using Test.SevenTiny.Bantina.Bankinate.Helpers;

namespace Test.SevenTiny.Bantina.Bankinate.SqlDbTest.MySql
{
    [DataBase("SevenTinyTest")]
    public class MySqlDb : MySqlDbContext<MySqlDb>
    {
        public MySqlDb() : base(ConnectionStrings.Get("mysql39901"))
        {
            
        }
    }
}
