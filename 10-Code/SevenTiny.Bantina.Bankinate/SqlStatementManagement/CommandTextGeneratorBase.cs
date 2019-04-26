using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SevenTiny.Bantina.Bankinate.SqlStatementManagement
{
    /// <summary>
    /// 命令生成规则的基类
    /// </summary>
    internal abstract class CommandTextGeneratorBase
    {
        public CommandTextGeneratorBase(SqlDbContext _dbContext)
        {
            DbContext = _dbContext;
        }

        //context
        protected SqlDbContext DbContext;
        protected string _where;
        protected string _orderBy;
        protected int _pageIndex;
        protected int _pageSize;
        protected string _alias;
        protected List<string> _columns;

        #region 设置关键字
        public abstract void SetWhere<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class;
        public abstract void SetOrderBy<TEntity>(Expression<Func<TEntity, object>> orderBy, bool isDesc) where TEntity : class;
        public abstract void SetPage(int pageIndex, int pageSize);
        public abstract void SetAlias(string alias);
        public abstract void SetColumns<TEntity>(Expression<Func<TEntity, object>> columns) where TEntity : class;
        #endregion

        //Cache properties by type
        private static ConcurrentDictionary<Type, PropertyInfo[]> _propertiesDic = new ConcurrentDictionary<Type, PropertyInfo[]>();
        protected static PropertyInfo[] GetPropertiesDicByType(Type type)
        {
            _propertiesDic.AddOrUpdate(type, type.GetProperties());
            return _propertiesDic[type];
        }

        public abstract string Add<TEntity>(TEntity entity) where TEntity : class;
        public abstract string Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        public abstract string Update<TEntity>(TEntity entity, out Expression<Func<TEntity, bool>> filter) where TEntity : class;
        public abstract string Delete<TEntity>(TEntity entity) where TEntity : class;
        public abstract string Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

        #region Queryable Methods
        public abstract string Limit(int count);
        public abstract string QueryableCount<TEntity>(string alias, string where) where TEntity : class;
        public abstract string QueryableAny<TEntity>(string alias, string where) where TEntity : class;
        public abstract string QueryableQuery<TEntity>(List<string> columns, string alias, string where, string orderBy, string top) where TEntity : class;
        //目前queryablePaging是最终的结果了
        public abstract string QueryablePaging<TEntity>(List<string> columns, string alias, string where, string orderBy, int pageIndex, int pageSize) where TEntity : class;
        #endregion

    }
}
