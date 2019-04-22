using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate.CacheManagement
{
    internal abstract class CacheManagerBase
    {
        protected SqlDbContext DbContext;

        protected CacheManagerBase(SqlDbContext context)
        {
            DbContext = context;
            CacheStorageManager = new CacheStorageManager(context);
        }

        protected CacheStorageManager CacheStorageManager { get; set; }
    }
}
