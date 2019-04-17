using MySql.Data.MySqlClient;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SevenTiny.Bantina.Bankinate
{
    public abstract class MySqlDbContext<TDataBase> : SqlDbContext, IDisposable where TDataBase : class
    {
        protected MySqlDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
        {
            DataBaseType = DataBaseType.MySql;

            CreateDbConnection(connectionString_Write);
            CreateDbCommand();
            CreateDbDataAdapter();
            //初始化访问器
            AccessorInitializes();
        }

        protected override DbConnection DbConnection { get; set; }
        protected override DbCommand DbCommand { get; set; }
        protected override DbDataAdapter DbDataAdapter { get; set; }

        private void CreateDbConnection(string connectionString)
        {
            DbConnection = new MySqlConnection(connectionString);
        }
        private void CreateDbCommand()
        {
            DbCommand = new MySqlCommand();
            DbCommand.Connection = this.DbConnection;

            //记得在使用的地方给类型和sql语句赋值
        }
        private void CreateDbDataAdapter()
        {
            DbDataAdapter = new MySqlDataAdapter();
            DbDataAdapter.SelectCommand = this.DbCommand;
        }
        /// <summary>
        /// 命令参数初始化
        /// </summary>
        protected override void ParameterInitializes()
        {
            if (Parameters != null && Parameters.Any())
            {
                DbCommand.Parameters.Clear();
                Parameters.Foreach(t => DbCommand.Parameters.Add(new MySqlParameter(t.Key, t.Value ?? DBNull.Value)));
            }
        }

        public new void Dispose()
        {
            //释放资源
            if (this.DbDataAdapter != null)
                this.DbDataAdapter.Dispose();

            if (this.DbCommand != null)
                this.DbCommand.Dispose();

            if (this.DbConnection.State == ConnectionState.Open)
                this.DbConnection.Close();
            if (this.DbConnection != null)
                this.DbConnection.Dispose();

            //调用基类的Dispose
            base.Dispose();
        }
    }
}
