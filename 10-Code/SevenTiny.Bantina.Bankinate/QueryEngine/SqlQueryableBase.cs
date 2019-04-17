/*********************************************************
* CopyRight: 7TINY CODE BUILDER. 
* Version: 5.0.0
* Author: 7tiny
* Address: Earth
* Create: 1/8/2019, 5:31:04 PM
* Modify: 
* E-mail: dong@7tiny.com | sevenTiny@foxmail.com 
* GitHub: https://github.com/sevenTiny 
* Personal web site: http://www.7tiny.com 
* Technical WebSit: http://www.cnblogs.com/7tiny/ 
* Description: 懒加载查询配置基类
* Thx , Best Regards ~
*********************************************************/
using SevenTiny.Bantina.Bankinate.DbContexts;
using System.Collections.Generic;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// SqlQueryable的相关配置信息
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class SqlQueryableBase
    {
        public SqlQueryableBase(SqlDbContext _dbContext)
        {
            DbContext = _dbContext;
        }

        //context
        protected SqlDbContext DbContext;

        //query info
        public string SqlStatement => DbContext.DbCommand.CommandText;
        public string TableName => DbContext.TableName;
        public IDictionary<string, object> Parameters => DbContext.Parameters;
    }
}
