using SevenTiny.Bantina.Bankinate.Configs;
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

        public DataSet ToDataSet()
        {
            return QueryExecutor.ExecuteDataSet(DbContext);
        }
        public object ToData()
        {
            return QueryExecutor.ExecuteScalar(DbContext);
        }
        public TEntity ToOne<TEntity>() where TEntity : class
        {
            return QueryExecutor.ExecuteEntity<TEntity>(DbContext);
        }

        public List<TEntity> ToList<TEntity>() where TEntity : class
        {
            return QueryExecutor.ExecuteList<TEntity>(DbContext);
        }
    }
}
