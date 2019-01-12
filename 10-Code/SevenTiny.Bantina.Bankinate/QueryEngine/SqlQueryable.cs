﻿/*********************************************************
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
using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.DataAccessEngine;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Helpers;
using SevenTiny.Bantina.Bankinate.SqlStatementManager;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    public class SqlQueryable<TEntity> : QueryableBase<TEntity> where TEntity : class
    {
        public SqlQueryable(DbContext dbContext) : base(dbContext) { }

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
            _columns = SqlGenerator.QueryableSelect(_dbContext, columns);
            return this;
        }

        /// <summary>
        /// 取最前面的count行，该方法不能和分页方法连用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public SqlQueryable<TEntity> Limit(int count)
        {
            switch (_dbContext.DataBaseType)
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
                var result = DbCacheManager.GetEntities(_dbContext, _where, () =>
                {
                    _dbContext.SqlStatement = SqlGenerator.QueryablePaging<TEntity>(
                        _dbContext,
                        _columns,
                        _alias,
                        SqlGenerator.QueryableWhere(_dbContext, _where),
                        SqlGenerator.QueryableOrderBy(_dbContext, _orderby, _isDesc),
                        _pageIndex,
                        _pageSize);
                    return DbHelper.ExecuteList<TEntity>(_dbContext);
                });
                return result;
            }
            else
            {
                return DbCacheManager.GetEntities(_dbContext, _where, () =>
                {
                    _dbContext.SqlStatement = SqlGenerator.QueryableQuery<TEntity>(
                        _dbContext,
                        _columns,
                        _alias,
                        SqlGenerator.QueryableWhere(_dbContext, _where),
                        SqlGenerator.QueryableOrderBy(_dbContext, _orderby, _isDesc),
                        _top);
                    return DbHelper.ExecuteList<TEntity>(_dbContext);
                });
            }
        }

        public override TEntity ToEntity()
        {
            MustExistCheck();
            ReSetTableName();

            Limit(1);

            return DbCacheManager.GetEntity(_dbContext, _where, () =>
            {
                _dbContext.SqlStatement = SqlGenerator.QueryableQuery<TEntity>(
                    _dbContext,
                    _columns,
                    _alias,
                    SqlGenerator.QueryableWhere(_dbContext, _where),
                    SqlGenerator.QueryableOrderBy(_dbContext, _orderby, _isDesc),
                    _top);
                return DbHelper.ExecuteEntity<TEntity>(_dbContext);
            });
        }

        public override int ToCount()
        {
            MustExistCheck();
            ReSetTableName();

            return DbCacheManager.GetCount(_dbContext, _where, () =>
            {
                _dbContext.SqlStatement = SqlGenerator.QueryableQueryCount<TEntity>(
                    _dbContext,
                    _alias,
                    SqlGenerator.QueryableWhere(_dbContext, _where));

                return Convert.ToInt32(DbHelper.ExecuteScalar(_dbContext));
            });
        }
    }
}
