using MySql.Data.MySqlClient;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using SevenTiny.Bantina.Bankinate.MySql.SqlStatementManagement;
using System;
using System.Data.Common;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MySqlDbContext<TDataBase> : SqlDbContext, IDisposable where TDataBase : class
    {
        protected MySqlDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
        {
            DataBaseType = DataBaseType.MySql;
        }

        internal override void CreateDbConnection(string connectionString)
        {
            DbConnection = new MySqlConnection(connectionString);
        }
        internal override void CreateDbCommand()
        {
            DbCommand = new MySqlCommand();
            DbCommand.Connection = this.DbConnection;
        }
        internal override void CreateDbDataAdapter()
        {
            DbDataAdapter = new MySqlDataAdapter();
            DbDataAdapter.SelectCommand = this.DbCommand;
        }
        internal override void CreateCommandTextGenerator() => CommandTextGenerator = new MySqlCommandTextGenerator(this);
        /// <summary>
        /// 命令参数初始化
        /// </summary>
        internal override void ParameterInitializes()
        {
            if (Parameters != null && Parameters.Any())
            {
                DbCommand.Parameters.Clear();
                Parameters.Foreach(t => DbCommand.Parameters.Add(new MySqlParameter(t.Key, t.Value ?? DBNull.Value)));
            }
        }

        public new void Dispose()
        {
            //调用基类的Dispose
            base.Dispose();
        }
    }
}
