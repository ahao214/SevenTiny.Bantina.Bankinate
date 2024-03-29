﻿/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/8/2019, 5:31:04 PM
* Modify: 2019年4月26日16点35分
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
        private string Alias => _where.Parameters[0].Name;

        public SqlQueryable(SqlDbContext _dbContext) : base(_dbContext)
        {
            DbContext.DbCommand.CommandType = CommandType.Text;
        }

        public SqlQueryable<TEntity> Where(Expression<Func<TEntity, bool>> filter)
        {
            if (_where != null)
                _where = _where.And(filter);
            else
                _where = filter;
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
                pageIndex = 0;

            if (pageSize <= 0)
                pageSize = 10;

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
            _columns = columns;
            return this;
        }

        /// <summary>
        /// 取最前面的count行，该方法不能和分页方法连用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public SqlQueryable<TEntity> Limit(int count)
        {
            DbContext.CommandTextGenerator.SetLimit(count);
            return this;
        }

        public override List<TEntity> ToList()
        {
            MustExistCheck();
            ReSetTableName();

            DbContext.CommandTextGenerator.SetAlias(Alias);
            DbContext.CommandTextGenerator.SetColumns(_columns);
            DbContext.CommandTextGenerator.SetWhere(_where);
            DbContext.CommandTextGenerator.SetOrderBy(_orderby, _isDesc);

            if (_isPaging)
            {
                DbContext.CommandTextGenerator.SetPage(_pageIndex, _pageSize);
                DbContext.CommandTextGenerator.QueryablePaging<TEntity>();
            }
            else
            {
                DbContext.CommandTextGenerator.QueryableQuery<TEntity>();
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

            DbContext.CommandTextGenerator.SetAlias(Alias);
            DbContext.CommandTextGenerator.SetColumns(_columns);
            DbContext.CommandTextGenerator.SetWhere(_where);
            DbContext.CommandTextGenerator.SetOrderBy(_orderby, _isDesc);
            Limit(1);
            DbContext.CommandTextGenerator.QueryableQuery<TEntity>();

            return DbContext.DbCacheManager.GetEntity(_where, () =>
           {
               return DbContext.QueryExecutor.ExecuteEntity<TEntity>();
           });
        }

        public override long Count()
        {
            MustExistCheck();
            ReSetTableName();

            DbContext.CommandTextGenerator.SetAlias(Alias);
            DbContext.CommandTextGenerator.SetWhere(_where);
            DbContext.CommandTextGenerator.QueryableCount<TEntity>();

            return DbContext.DbCacheManager.GetCount(_where, () =>
           {
               return Convert.ToInt32(DbContext.QueryExecutor.ExecuteScalar());
           });
        }

        public override bool Any()
        {
            MustExistCheck();
            ReSetTableName();

            DbContext.CommandTextGenerator.SetAlias(Alias);
            DbContext.CommandTextGenerator.SetWhere(_where);
            DbContext.CommandTextGenerator.QueryableAny<TEntity>(); //内部 Limit(1)

            return DbContext.DbCacheManager.GetCount(_where, () =>
            {
                return Convert.ToInt32(DbContext.QueryExecutor.ExecuteScalar());
            }) > 0;
        }
    }
}
