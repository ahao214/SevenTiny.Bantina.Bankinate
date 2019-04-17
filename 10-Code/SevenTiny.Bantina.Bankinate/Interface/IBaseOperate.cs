using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 基础操作Api
    /// </summary>
    public interface IBaseOperate
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddAsync<TEntity>(TEntity entity) where TEntity : class;

        void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        void UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;

        void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        void DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    }
}
