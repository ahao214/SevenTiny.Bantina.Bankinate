using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.SqlDataAccess;
using System.Collections.Generic;
using System.Data;

namespace SevenTiny.Bantina.Bankinate
{
    public class SqlQueryable : SqlQueryableBase
    {
        public SqlQueryable(SqlDbContext _dbContext) : base(_dbContext) { }

        public DataSet ToDataSet(string sqlStatement, IDictionary<string, object> parms = null)
        {
            dbContext.SqlStatement = sqlStatement;
            dbContext.Parameters = parms;
            return QueryExecutor.ExecuteDataSet(dbContext);
        }
        public object ToData(string sqlStatement, IDictionary<string, object> parms = null)
        {
            dbContext.SqlStatement = sqlStatement;
            dbContext.Parameters = parms;
            return QueryExecutor.ExecuteScalar(dbContext);
        }
        public TEntity ToOne<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class
        {
            dbContext.SqlStatement = sqlStatement;
            dbContext.Parameters = parms;
            return QueryExecutor.ExecuteEntity<TEntity>(dbContext);
        }
        public List<TEntity> ToList<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class
        {
            dbContext.SqlStatement = sqlStatement;
            dbContext.Parameters = parms;
            return QueryExecutor.ExecuteList<TEntity>(dbContext);
        }
    }
}
