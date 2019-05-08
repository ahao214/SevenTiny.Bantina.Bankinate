﻿using System.Collections.Generic;

namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 执行sql语句扩展Api
    /// </summary>
    public interface IExecuteSqlOperate
    {
        int ExecuteSql(string sqlStatement, IDictionary<string, object> parms = null);
    }
}
