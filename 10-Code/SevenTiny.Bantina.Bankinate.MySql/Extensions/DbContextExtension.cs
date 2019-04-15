using SevenTiny.Bantina.Bankinate.DbContexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SevenTiny.Bantina.Bankinate.MySql.Extensions
{
    public static class DbContextExtension
    {
        public static DbContext UseMySql(this DbContext dbContext, string connectionString_Write, params string[] connectionStrings_Read)
        {
            dbContext.DataBaseType = DataBaseType.MySql;
            dbContext.ConnectionString_Write = connectionString_Write;
            dbContext.ConnectionStrings_Read = connectionStrings_Read;
            return dbContext;
        }
    }
}
