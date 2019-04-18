using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.SqlDataAccess;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// SQL弱类型复杂查询器
    /// </summary>
    public class SqlQueryable : SqlQueryableBase
    {
        public SqlQueryable(SqlDbContext _dbContext) : base(_dbContext)
        {
            DbContext.DbCommand.CommandType = CommandType.Text;
        }

        public void Execute() => QueryExecutor.ExecuteNonQuery(DbContext);
        public async Task<int> ExecuteAsync() => await QueryExecutor.ExecuteNonQueryAsync(DbContext);

        public DataSet ToDataSet() => QueryExecutor.ExecuteDataSet(DbContext);
        public object ToData() => QueryExecutor.ExecuteScalar(DbContext);
        public TEntity ToOne<TEntity>() where TEntity : class => QueryExecutor.ExecuteEntity<TEntity>(DbContext);
        public List<TEntity> ToList<TEntity>() where TEntity : class => QueryExecutor.ExecuteList<TEntity>(DbContext);
    }
}
