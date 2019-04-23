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
            return DbContext.QueryExecutor.ExecuteDataSet();
        }
        public object ToData()
        {
            return DbContext.QueryExecutor.ExecuteScalar();
        }
        public TEntity ToOne<TEntity>() where TEntity : class
        {
            return DbContext.QueryExecutor.ExecuteEntity<TEntity>();
        }

        public List<TEntity> ToList<TEntity>() where TEntity : class
        {
            return DbContext.QueryExecutor.ExecuteList<TEntity>();
        }
    }
}
