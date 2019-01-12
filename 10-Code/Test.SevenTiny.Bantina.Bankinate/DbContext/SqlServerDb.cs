﻿using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    [DataBase("SevenTinyTest")]
    public class SqlServerDb : SqlServerDbContext<SqlServerDb>
    {
        public SqlServerDb() : base("Data Source=.;Initial Catalog=SevenTinyTest;Integrated Security=True")
        {
            
        }
    }
}
