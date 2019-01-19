using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test.SevenTiny.Bantina.Bankinate.DbContext;
using Test.SevenTiny.Bantina.Bankinate.Model;
using Xunit;

namespace Test.SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// 关系型数据库持久化测试
    /// </summary>
    public class SqlDbPersistenceTest
    {
        //这里切换对应的关系型数据库上下文
        MySqlDb Db => new MySqlDb();
        //SqlServerDb Db => new SqlServerDb();

        [Theory]
        [InlineData(9999)]
        public void Add(int value)
        {
            OperateTestModel model = new OperateTestModel
            {
                IntKey = value,
                StringKey = "AddTest"
            };
            model.IntKey = value;
            Db.Add<OperateTestModel>(model);
        }

        [Theory]
        [InlineData(9999)]
        public void Update(int value)
        {
            OperateTestModel model = Db.QueryOne<OperateTestModel>(t => t.IntKey == value);
            //model.Id = value;   //自增的主键不应该被修改,如果用这种方式进行修改，给Id赋值就会导致修改不成功，因为条件是用第一个主键作为标识修改的
            model.Key2 = value;
            model.StringKey = $"UpdateTest_{value}";
            model.IntNullKey = value;
            model.DateTimeNullKey = DateTime.Now;
            model.DateNullKey = DateTime.Now.Date;
            model.DoubleNullKey = model.IntNullKey;
            model.FloatNullKey = model.IntNullKey;
            Db.Update<OperateTestModel>(model);
        }

        [Theory]
        [InlineData(9999)]
        public void DeleteWhere(int value)
        {
            Db.Delete<OperateTestModel>(t => t.IntKey == value);
        }

        [Theory]
        [InlineData(9999)]
        public void DeleteEntity(int value)
        {
            OperateTestModel model = Db.QueryOne<OperateTestModel>(t => t.IntKey == value);
            Db.Delete<OperateTestModel>(model);
        }
    }
}
