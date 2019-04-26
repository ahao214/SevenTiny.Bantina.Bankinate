namespace Test.Common
{
    public class ConnectionStringHelper
    {
        public static string ConnectionString_Write = "123";//ConnectionStrings.Get("mysql39901")
        public static string[] ConnectionStrings_Read = 
            new[] {
                ConnectionString_Write,
                ConnectionString_Write,
                ConnectionString_Write
            };
    }
}
