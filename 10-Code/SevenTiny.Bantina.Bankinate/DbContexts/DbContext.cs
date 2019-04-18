using SevenTiny.Bantina.Bankinate.Cache;
using SevenTiny.Bantina.Bankinate.Configs;
using SevenTiny.Bantina.Bankinate.ConnectionManagement;
using SevenTiny.Bantina.Bankinate.Helpers;
using System;
using System.Linq;

namespace SevenTiny.Bantina.Bankinate.DbContexts
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public abstract class DbContext : IDisposable
    {
        protected DbContext(string connectionString_Write, params string[] connectionStrings_Read)
        {
            if (string.IsNullOrEmpty(connectionString_Write))
                throw new ArgumentNullException(nameof(connectionString_Write), "argument can not be null");

            if (ConnectionManager == null)
                ConnectionManager = new ConnectionManager(connectionString_Write, connectionStrings_Read);
        }

        #region Database Control 数据库管理
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DataBaseType { get; protected set; }
        /// <summary>
        /// 连接管理器
        /// </summary>
        public ConnectionManager ConnectionManager { get; }
        /// <summary>
        /// 真实执行持久化操作开关，如果为false，则只执行准备动作，不实际操作数据库（友情提示：测试框架代码执行情况可以将其关闭）
        /// </summary>
        public bool OpenRealExecutionSaveToDb { get; protected set; } = true;
        #endregion

        #region Cache Control 缓存管理
        /// <summary>
        /// 一级缓存
        /// 查询条件级别的缓存（filter），可以暂时缓存根据查询条件查询到的数据
        /// 如果开启二级缓存，且当前操作对应的表已经在二级缓存里，则不进行条件缓存
        /// </summary>
        public bool OpenQueryCache { get; protected set; } = false;
        /// <summary>
        /// 二级缓存
        /// 配置表缓存标签对整张数据库表进行缓存
        /// </summary>
        public bool OpenTableCache { get; protected set; } = false;
        /// <summary>
        /// 查询缓存的默认缓存时间
        /// </summary>
        private TimeSpan _QueryCacheExpiredTimeSpan = BankinateConst.QueryCacheExpiredTimeSpan;
        public TimeSpan QueryCacheExpiredTimeSpan
        {
            get { return _QueryCacheExpiredTimeSpan; }
            protected set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                _QueryCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 表缓存的缓存时间
        /// </summary>
        private TimeSpan _TableCacheExpiredTimeSpan = BankinateConst.TableCacheExpiredTimeSpan;
        public TimeSpan TableCacheExpiredTimeSpan
        {
            get { return _TableCacheExpiredTimeSpan; }
            protected set
            {
                if (value > MaxExpiredTimeSpan)
                {
                    MaxExpiredTimeSpan = value;
                }
                _TableCacheExpiredTimeSpan = value;
            }
        }
        /// <summary>
        /// 每张表一级缓存的最大个数，超出数目将会按从早到晚的顺序移除缓存键
        /// </summary>
        public int QueryCacheMaxCountPerTable { get; protected set; } = BankinateConst.QueryCacheMaxCountPerTable;
        /// <summary>
        /// 数据是否从缓存中获取
        /// </summary>
        public bool IsFromCache { get; internal set; } = false;
        /// <summary>
        /// Cache 存储媒介,默认本地缓存
        /// </summary>
        public CacheMediaType CacheMediaType { get; protected set; } = BankinateConst.CacheMediaType;
        /// <summary>
        /// Cache 第三方存储媒介服务地址
        /// </summary>
        public string CacheMediaServer { get; protected set; }
        /// <summary>
        /// 最大的缓存时间（用于缓存缓存键）
        /// </summary>
        internal TimeSpan MaxExpiredTimeSpan { get; set; } = BankinateConst.CacheKeysMaxExpiredTime;

        ///// <summary>
        ///// 清空全部缓存
        ///// </summary>
        //public void FlushAllCache() => DbCacheManager.FlushAllCache(this);
        ///// <summary>
        ///// 清空一级缓存
        ///// </summary>
        //public void FlushQueryCache() => QueryCacheManager.FlushAllCache(this);
        ///// <summary>
        ///// 清空二级缓存
        ///// </summary>
        //public void FlushTableCache() => TableCacheManager.FlushAllCache(this);
        #endregion

        #region Validate Control 校验管理
        /// <summary>
        /// 属性值校验开关，如开启，则Add/Update等操作会校验输入的值是否满足特性标签标识的条件
        /// </summary>
        public bool OpenPropertyDataValidate { get; protected set; } = false;
        #endregion

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
