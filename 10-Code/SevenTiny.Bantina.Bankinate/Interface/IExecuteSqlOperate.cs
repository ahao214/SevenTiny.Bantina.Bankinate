using System.Collections.Generic;
using System.Data;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 执行sql语句扩展Api
    /// </summary>
    public interface IExecuteSqlOperate
    {
        void ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null);
        void ExecuteSqlAsync(string sqlStatement, IDictionary<string, object> parms = null);
        DataSet ExecuteQueryDataSetSql(string sqlStatement, IDictionary<string, object> parms = null);
        object ExecuteQueryOneDataSql(string sqlStatement, IDictionary<string, object> parms = null);
        TEntity ExecuteQueryOneSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class;
        List<TEntity> ExecuteQueryListSql<TEntity>(string sqlStatement, IDictionary<string, object> parms = null) where TEntity : class;
    }
}
