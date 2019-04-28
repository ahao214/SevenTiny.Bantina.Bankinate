using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 基础操作Api
    /// </summary>
    public interface IBaseOperate
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;

        void Update<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;
        Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> filter, TEntity entity) where TEntity : class;

        void Delete<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
    }
}
