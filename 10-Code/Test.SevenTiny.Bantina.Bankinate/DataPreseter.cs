using System;
using System.Collections.Generic;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Test.SevenTiny.Bantina.Bankinate.SqlDbTest.MySql;
using Test.SevenTiny.Bantina.Bankinate.SqlDbTest.SqlServer;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 数据预置类
    /// </summary>
    public class DataPreseter
    {
        //[Fact]
        [Trait("desc", "初始化MySql测试数据")]
        public void InitMySqlData()
        {
            using (var db = new MySqlDb())
            {
                //清空所有数据,并重置索引
                db.ExecuteSql("truncate table " + db.GetTableName<OperateTestModel>());

                //预置测试数据
                List<OperateTestModel> models = new List<OperateTestModel>();
                for (int i = 1; i < 1001; i++)
                {
                    models.Add(new OperateTestModel
                    {
                        Key2 = i,
                        StringKey = $"test_{i}",
                        IntKey = i,
                        IntNullKey = null,
                        DateNullKey = DateTime.Now.Date,
                        DateTimeNullKey = DateTime.Now,
                        DoubleNullKey = i,
                        FloatNullKey = i
                    });
                }
                db.Add<OperateTestModel>(models);
            }
            Assert.True(true);
        }

        //[Fact]
        [Trait("desc", "初始化SqlServer测试数据")]
        public void InitSqlServerData()
        {
            using (var db = new SqlServerDb())
            {
                //清空所有数据,并重置索引
                db.ExecuteSql("truncate table " + db.GetTableName<OperateTestModel>());

                //预置测试数据
                List<OperateTestModel> models = new List<OperateTestModel>();
                for (int i = 1; i < 1001; i++)
                {
                    models.Add(new OperateTestModel
                    {
                        Key2 = i,
                        StringKey = $"test_{i}",
                        IntKey = i,
                        IntNullKey = null,
                        DateNullKey = DateTime.Now.Date,
                        DateTimeNullKey = DateTime.Now,
                        DoubleNullKey = i,
                        FloatNullKey = i
                    });
                }
                db.Add<OperateTestModel>(models);
            }
            Assert.True(true);
        }
    }
}
