﻿using SevenTiny.Bantina.Bankinate;
using SevenTiny.Bantina.Bankinate.Attributes;
using System;
using System.Collections.Generic;
using Test.Common;
using Test.Common.Model;
using Xunit;

namespace Test.MySql
{
    /// <summary>
    /// 数据预置类
    /// </summary>
    public class DataPreseter
    {
        [DataBase("SevenTinyTest")]
        private class DataPreseterDb : MySqlDbContext<DataPreseterDb>
        {
            public DataPreseterDb() : base(ConnectionStringHelper.ConnectionString_Write, ConnectionStringHelper.ConnectionStrings_Read)
            {

            }
        }

        //[Fact]
        [Trait("desc", "初始化测试数据")]
        public void InitData()
        {
            return;
            using (var db = new DataPreseterDb())
            {
                //清空所有数据,并重置索引
                db.ExecuteSql("truncate table " + db.GetTableName<OperateTestModel>());

                //预置测试数据
                List<OperateTestModel> models = new List<OperateTestModel>();
                for (int i = 1; i < 1001; i++)
                {
                    db.Add<OperateTestModel>(new OperateTestModel
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
            }
            Assert.True(true);
        }
    }
}
