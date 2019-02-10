using StackExchange.Redis;
using System;
using System.Threading;

namespace SevenTiny.Bantina.Bankinate.Helpers.Redis
{
    internal abstract class RedisServerManager
    {
        protected ConnectionMultiplexer Redis { get; set; }
        protected IDatabase Db { get; set; }
        private string KeySpace { get; set; }

        protected RedisServerManager(string keySpace, string server)
        {
            KeySpace = keySpace;
            //set establish retry mechanism (3 times)
            int retryCount = 2;
            while (true)
            {
                try
                {
                    Redis = ConnectionMultiplexer.Connect(server);
                    Db = Redis.GetDatabase();
                    break;
                }
                catch (Exception ex)
                {
                    if (retryCount > 0)
                    {
                        retryCount--;
                        Thread.Sleep(1000);
                        continue;
                    }
                    throw new TimeoutException($"redis init timeout,server server reject or other.ex{ex.ToString()}");
                }
            }
        }
    }
}
