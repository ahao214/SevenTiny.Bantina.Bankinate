using System;
using System.Collections.Generic;
using System.Text;
using SevenTiny.Bantina.Bankinate.DbContexts;

namespace Test.SevenTiny.Bantina.Bankinate.SqlDbTest.MySql
{
    /// <summary>
    /// mysql操作测试
    /// </summary>
    public class SqlServerOperationTest : SqlDbOperationTest<MySqlDb>
    {
        public override SqlDbContext<MySqlDb> Db => new MySqlDb();
    }
}
