using System;
using System.Collections.Generic;
using System.Text;
using SevenTiny.Bantina.Bankinate.DbContexts;

namespace Test.SevenTiny.Bantina.Bankinate.SqlDbTest.SqlServer
{
    /// <summary>
    /// sqlserver操作测试
    /// </summary>
    public class SqlServerOperationTest : SqlDbOperationTest<SqlServerDb>
    {
        public override SqlDbContext<SqlServerDb> Db => new SqlServerDb();
    }
}
