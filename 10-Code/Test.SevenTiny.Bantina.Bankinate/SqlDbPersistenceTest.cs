using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.DbContext;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 关系型数据库持久化测试
    /// </summary>
    public class SqlDbPersistenceTest
    {
        //这里切换对应的关系型数据库上下文
        MySqlDb Db => new MySqlDb();
        //SqlServerDb Db => new SqlServerDb();

        
    }
}
