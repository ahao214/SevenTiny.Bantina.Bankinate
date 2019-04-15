using System;
using System.Collections.Generic;
using System.Text;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MySqlDbContext<TDataBase> : SqlDbContext<TDataBase> where TDataBase : class
    {
        protected MySqlDbContext(string connectionString) : this(connectionString, connectionString) { }
        protected MySqlDbContext(string connectionString_ReadWrite, string connectionString_Read) : base(DataBaseType.MySql, connectionString_ReadWrite, connectionString_Read) { }
    }
}
