using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;

namespace Test.SevenTiny.Bantina.Bankinate.DbContext
{
    [DataBase("local")]
    public class MongoDb : MongoDbContext<MongoDb>
    {
        public MongoDb() : base(ConnectionStrings.Get("mongodb39911"))
        {

        }
    }
}
