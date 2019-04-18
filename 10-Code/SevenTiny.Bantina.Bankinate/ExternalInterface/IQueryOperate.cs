using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 基础查询Api
    /// </summary>
    public interface IQueryOperate
    {
        List<TEntity> QueryList<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        bool QueryExist<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        int QueryCount<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        TEntity QueryOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    }
}
