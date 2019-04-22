using SevenTiny.Bantina.Bankinate.DbContexts;

namespace SevenTiny.Bantina.Bankinate.CacheManagement
{
    internal abstract class CacheManagerBase
    {
        protected DbContext DbContext;

        protected CacheManagerBase(DbContext context)
        {
            DbContext = context;
            CacheStorageManager = new CacheStorageManager(context);
        }

        protected CacheStorageManager CacheStorageManager { get; set; }
    }
}
