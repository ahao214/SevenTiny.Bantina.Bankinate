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

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    public abstract class SqlDbContext : DbContext, IDbContext
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
        /// Sql语句
        /// </summary>
        public string SqlStatement { get; internal set; }
        /// <summary>
        /// 数据库连接管理器
        /// </summary>
        protected abstract DbConnection DbConnection { get; set; }
        /// <summary>
        /// 命令管理器
        /// </summary>
        protected internal abstract DbCommand DbCommand { get; set; }
        /// <summary>
        /// 集合访问器
        /// </summary>
        protected internal abstract DbDataAdapter DbDataAdapter { get; set; }
        /// <summary>
        /// 初始化访问器
        /// </summary>
        protected void AccessorInitializes()
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
        protected internal abstract void ParameterInitializes();

        /// <summary>
        /// 参数化查询参数
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

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

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Add(this, entity);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Add(this, entity);
        }
        public void AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Add(this, entity);
            QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Add(this, entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            SqlGenerator.Delete(this, entity);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Delete(this, entity);
        }
        public void DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            SqlGenerator.Delete(this, entity);
            QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Delete(this, entity);
        }
        public void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.Delete(this, filter);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Delete(this, filter);
        }
        public void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            SqlGenerator.Delete(this, filter);
            QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Delete(this, filter);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, entity, out Expression<Func<TEntity, bool>> filter);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public void UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, entity, out Expression<Func<TEntity, bool>> filter);
            QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, filter, entity);
            QueryExecutor.ExecuteNonQuery(this);
            DbCacheManager.Update(this, entity, filter);
        }
        public void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class
        {
            PropertyDataValidator.Verify(this, entity);
            SqlGenerator.Update(this, filter, entity);
            QueryExecutor.ExecuteNonQueryAsync(this);
            DbCacheManager.Update(this, entity, filter);
        }

        /// <summary>
        /// 支持复杂高效查询的查询入口
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public SqlQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return new SqlQueryable<TEntity>(this);
        }
        public SqlQueryable Queryable()
        {
            return new SqlQueryable(this);
        }

        public void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null)
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            QueryExecutor.ExecuteNonQuery(this);
        }
        public void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null)
        {
            SqlStatement = sqlStatement;
            Parameters = parms;
            QueryExecutor.ExecuteNonQueryAsync(this);
        }
    }
}
