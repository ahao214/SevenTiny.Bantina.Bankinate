﻿using SevenTiny.Bantina.Bankinate;
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
}
