using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SevenTiny.Bantina.Bankinate.SqlStatementManager
{
    /// <summary>
    /// 命令生成规则的基类
    /// </summary>
    internal abstract class CommandTextGeneratorBase
    {
        //Cache properties by type
        private static ConcurrentDictionary<Type, PropertyInfo[]> _propertiesDic = new ConcurrentDictionary<Type, PropertyInfo[]>();
        protected static PropertyInfo[] GetPropertiesDicByType(Type type)
        {
            _propertiesDic.AddOrUpdate(type, type.GetProperties());
            return _propertiesDic[type];
        }

        public abstract string Add<TEntity>(SqlDbContext dbContext, TEntity entity) where TEntity : class;
        public abstract string Update<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        public abstract string Update<TEntity>(SqlDbContext dbContext, TEntity entity, out Expression<Func<TEntity, bool>> filter) where TEntity : class;
        public abstract string Delete<TEntity>(SqlDbContext dbContext, TEntity entity) where TEntity : class;
        public abstract string Delete<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, bool>> filter) where TEntity : class;

        #region Queryable Methods
        public abstract string QueryableWhere<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, bool>> filter) where TEntity : class;
        public abstract string QueryableOrderBy<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, object>> orderBy, bool isDESC) where TEntity : class;
        public abstract List<string> QueryableSelect<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, object>> columns) where TEntity : class;
        public abstract string QueryableQueryCount<TEntity>(SqlDbContext dbContext, string alias, string where) where TEntity : class;
        public abstract string QueryableQuery<TEntity>(SqlDbContext dbContext, List<string> columns, string alias, string where, string orderBy, string top) where TEntity : class;
        //目前queryablePaging是最终的结果了
        public abstract string QueryablePaging<TEntity>(SqlDbContext dbContext, List<string> columns, string alias, string where, string orderBy, int pageIndex, int pageSize) where TEntity : class;
        #endregion

    }
}
