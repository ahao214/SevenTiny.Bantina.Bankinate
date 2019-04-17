using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.SqlDataAccess;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 存储过程弱类型复杂查询器
    /// </summary>
    public class StoredProcedureQueryable : SqlQueryableBase
    {
        public StoredProcedureQueryable(SqlDbContext _dbContext) : base(_dbContext)
        {
            DbContext.DbCommand.CommandType = CommandType.StoredProcedure;
        }

        public void ExecuteStoredProcedure() => QueryExecutor.ExecuteNonQuery(DbContext);
        public async Task<int> ExecuteStoredProcedureAsync() => await QueryExecutor.ExecuteNonQueryAsync(DbContext);
    }
}
