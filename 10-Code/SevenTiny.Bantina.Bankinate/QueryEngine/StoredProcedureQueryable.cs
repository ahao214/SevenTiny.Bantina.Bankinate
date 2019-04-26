using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.SqlDataAccess;
using System.Data;
using System.Threading.Tasks;

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
    }
}
