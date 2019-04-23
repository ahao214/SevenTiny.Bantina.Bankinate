/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/8/2019, 5:31:04 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 适合于Sql条件下的懒加载查询配置
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// SQL强类型复杂查询器
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SqlQueryable<TEntity> : SqlQueryableBase<TEntity> where TEntity : class
    {
        public SqlQueryable(SqlDbContext _dbContext) : base(_dbContext)
        {
            DbContext.DbCommand.CommandType = CommandType.Text;
        }

        /// <summary>
        /// 查询sql语句中表的别名
        /// </summary>
        protected string _alias;

        public SqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            if (_where != null)
                _where = _where.And(filter);
            else
                _where = filter;

            _alias = filter.Parameters[0].Name;
            return this;
        }

        public SqlQueryable<TEntity> OrderBy(Expression<Func<TEntity, object>> orderBy)
        {
            _orderby = orderBy;
            _isDesc = false;
            return this;
        }

        public SqlQueryable<TEntity> OrderByDescending(Expression<Func<TEntity, object>> orderBy)
        {
            _orderby = orderBy;
            _isDesc = true;
            return this;
        }

        public SqlQueryable<TEntity> Paging(int pageIndex, int pageSize)
        {
            _isPaging = true;

            if (pageIndex <= 0)
            {
                pageIndex = 0;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            _pageIndex = pageIndex;
            _pageSize = pageSize;

            return this;
        }

        /// <summary>
        /// 筛选具体的某几列
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public SqlQueryable<TEntity> Select(Expression<Func<TEntity, object>> columns)
        {
            _columns = DbContext.CommandTextGenerator.QueryableSelect(columns);
            return this;
        }

        /// <summary>
        /// 取最前面的count行，该方法不能和分页方法连用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public SqlQueryable<TEntity> Limit(int count)
        {
            switch (DbContext.DataBaseType)
            {
                case DataBaseType.SqlServer:
                    _top = $" TOP {count} "; break;
                case DataBaseType.MySql:
                    _top = $" LIMIT {count} "; break;
                case DataBaseType.Oracle:
                    break;
            }
            return this;
        }

        public override List<TEntity> ToList()
        {
            MustExistCheck();
            ReSetTableName();

            if (_isPaging)
            {
                DbContext.SqlStatement = DbContext.CommandTextGenerator.QueryablePaging<TEntity>(
                        _columns,
                        _alias,
                        DbContext.CommandTextGenerator.QueryableWhere(_where),
                        DbContext.CommandTextGenerator.QueryableOrderBy(_orderby, _isDesc),
                        _pageIndex,
                        _pageSize);
            }
            else
            {
                DbContext.SqlStatement = DbContext.CommandTextGenerator.QueryableQuery<TEntity>(
                        _columns,
                        _alias,
                        DbContext.CommandTextGenerator.QueryableWhere(_where),
                        DbContext.CommandTextGenerator.QueryableOrderBy(_orderby, _isDesc),
                        _top);
            }

            return DbContext.DbCacheManager.GetEntities(_where, () =>
            {
                return DbContext.QueryExecutor.ExecuteList<TEntity>();
            });
        }

        public override TEntity ToOne()
        {
            MustExistCheck();
            ReSetTableName();

            Limit(1);

            DbContext.SqlStatement = DbContext.CommandTextGenerator.QueryableQuery<TEntity>(
                    _columns,
                    _alias,
                    DbContext.CommandTextGenerator.QueryableWhere(_where),
                    DbContext.CommandTextGenerator.QueryableOrderBy(_orderby, _isDesc),
                    _top);

            return DbContext.DbCacheManager.GetEntity(_where, () =>
           {
               return DbContext.QueryExecutor.ExecuteEntity<TEntity>();
           });
        }

        public override int ToCount()
        {
            MustExistCheck();
            ReSetTableName();

            DbContext.SqlStatement = DbContext.CommandTextGenerator.QueryableQueryCount<TEntity>(
                    _alias,
                    DbContext.CommandTextGenerator.QueryableWhere(_where));

            return DbContext.DbCacheManager.GetCount(_where, () =>
           {
               return Convert.ToInt32(DbContext.QueryExecutor.ExecuteScalar());
           });
        }

        public override bool Any(Expression<Func<TEntity, bool>> filter)
        {
            return this.Where(filter).ToCount() > 0;
        }
    }
}
