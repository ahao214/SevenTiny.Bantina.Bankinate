using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.SqlDataAccess;
using SevenTiny.Bantina.Bankinate.Helpers;
using SevenTiny.Bantina.Bankinate.SqlStatementManager;
using SevenTiny.Bantina.Bankinate.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

//需要扩展的类型需要在此添加对应的程序集友元标识
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.MySql")]
[assembly: InternalsVisibleTo("SevenTiny.Bantina.Bankinate.SqlServer")]
namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class SqlDbContext : DbContext
    {
        protected SqlDbContext(string connectionString_Write, params string[] connectionStrings_Read) : base(connectionString_Write, connectionStrings_Read)
        {

        }

        /// <summary>
        /// 库名
        /// </summary>
        public string DataBaseName { get; protected set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; internal set; }
        /// <summary>
        /// Sql语句，获取或赋值命令行对象的CommandText参数
        /// </summary>
        public string SqlStatement
        {
            get => this.DbCommand?.CommandText;
            internal set
            {
                if (this.DbCommand == null)
                    throw new NullReferenceException("DbCommand is null,please initialize connection first!");
                this.DbCommand.CommandText = value;
            }
        }
        /// <summary>
        /// 参数化查询参数
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// 数据库连接管理器
        /// </summary>
        internal DbConnection DbConnection { get; set; }
        /// <summary>
        /// 命令管理器
        /// </summary>
        internal DbCommand DbCommand { get; set; }
        /// <summary>
        /// 结果集访问器
        /// </summary>
        internal DbDataAdapter DbDataAdapter { get; set; }
        /// <summary>
        /// 命令生成器
        /// </summary>
        internal CommandTextGeneratorBase CommandTextGenerator { get; set; }
        /// <summary>
        /// 创建连接管理器
        /// </summary>
        /// <param name="connectionString"></param>
        internal abstract void CreateDbConnection(string connectionString);
        /// <summary>
        /// 创建命令管理器
        /// </summary>
        internal abstract void CreateDbCommand();
        /// <summary>
        /// 创建结果集访问器
        /// </summary>
        internal abstract void CreateDbDataAdapter();
        /// <summary>
        /// 创建SQL生成器
        /// </summary>
        internal abstract void CreateCommandTextGenerator();
        /// <summary>
        /// 初始化访问器
        /// </summary>
        internal void AccessorInitializes()
        {
            //打开连接
            if (DbConnection.State != ConnectionState.Open)
                DbConnection.Open();

            //设置SqlCommand对象的属性值
            DbCommand.CommandTimeout = BankinateConst.CommandTimeout;
            DbCommand.CommandType = CommandType.Text;
        }
        /// <summary>
        /// 初始化查询参数
        /// </summary>
        internal abstract void ParameterInitializes();

        /// <summary>
        /// 根据实体获取表明
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public string GetTableName<TEntity>() where TEntity : class
            => TableAttribute.GetName(typeof(TEntity));

        /// <summary>
        /// 事务处理
        /// </summary>
        /// <param name="action"></param>
        public void Transaction(Action action)
        {
            try
            {
                this.DbCommand.Transaction = this.DbConnection.BeginTransaction();
                action();
                this.DbCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                this.DbCommand.Transaction.Rollback();
                throw ex;
            }
        }

        #region 强类型的执行操作API
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            this.CommandTextGenerator.Add(this, entity);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Add(this, entity);
        }
        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            this.CommandTextGenerator.Add(this, entity);
            await QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Add(this, entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            this.CommandTextGenerator.Delete(this, entity);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Delete(this, entity);
        }
        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            this.CommandTextGenerator.Delete(this, entity);
            await QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Delete(this, entity);
        }
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            this.CommandTextGenerator.Delete(this, filter);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Delete(this, filter);
        }
        public async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            this.CommandTextGenerator.Delete(this, filter);
            await QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Delete(this, filter);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            this.CommandTextGenerator.Update(this, entity, out Expression<Func<TEntity, bool>> filter);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            this.CommandTextGenerator.Update(this, entity, out Expression<Func<TEntity, bool>> filter);
            await QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            this.CommandTextGenerator.Update(this, filter, entity);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public async Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            this.CommandTextGenerator.Update(this, filter, entity);
            await QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Update(this, entity, filter);
        }
        #endregion

        #region 查询API
        /// <summary>
        /// SQL强类型复杂查询器
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public SqlQueryable<TEntity> Queryable<TEntity>() where TEntity : class => new SqlQueryable<TEntity>(this);
        /// <summary>
        /// SQL弱类型复杂查询器
        /// </summary>
        /// <returns></returns>
        public SqlQueryable Queryable(string sqlStatement, IDictionary<string, object> parms = null)
        {
            this.SqlStatement = sqlStatement;
            this.Parameters = parms;
            return new SqlQueryable(this);
        }
        /// <summary>
        /// 存储过程弱类型复杂查询器
        /// </summary>
        /// <returns></returns>
        public StoredProcedureQueryable StoredProcedureQueryable(string storedProcedureName, IDictionary<string, object> parms = null)
        {
            this.SqlStatement = storedProcedureName;
            this.Parameters = parms;
            return new StoredProcedureQueryable(this);
        }
        #endregion
    }
}
