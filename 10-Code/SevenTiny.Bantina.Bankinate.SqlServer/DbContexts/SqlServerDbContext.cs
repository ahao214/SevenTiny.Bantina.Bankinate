using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using SevenTiny.Bantina.Bankinate.SqlServer.SqlStatementManagement;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class SqlServerDbContext<TDataBase> : SqlDbContext, IDisposable where TDataBase : class
    {
        protected SqlServerDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
        {
            DataBaseType = DataBaseType.SqlServer;
        }

        internal override void CreateDbConnection(string connectionString)
        {
            DbConnection = new SqlConnection(connectionString);
        }
        internal override void CreateDbCommand()
        {
            DbCommand = new SqlCommand();
            DbCommand.Connection = this.DbConnection;
        }
        internal override void CreateDbDataAdapter()
        {
            DbDataAdapter = new SqlDataAdapter();
            DbDataAdapter.SelectCommand = this.DbCommand;
        }
        internal override void CreateCommandTextGenerator() => CommandTextGenerator = new SqlServerCommandTextGenerator(this);
        /// <summary>
        /// 命令参数初始化
        /// </summary>
        internal override void ParameterInitializes()
        {
            if (Parameters != null && Parameters.Any())
            {
                DbCommand.Parameters.Clear();
                Parameters.Foreach(t => DbCommand.Parameters.Add(new SqlParameter(t.Key, t.Value ?? DBNull.Value)));
            }
        }

        public new void Dispose()
        {
            //调用基类的Dispose
            base.Dispose();
        }
    }
}
