﻿using SevenTiny.Bantina.Bankinate.Attributes;
using SevenTiny.Bantina.Bankinate.DbContexts;
using SevenTiny.Bantina.Bankinate.Exceptions;
using SevenTiny.Bantina.Bankinate.Extensions;
using SevenTiny.Bantina.Bankinate.SqlStatementManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SevenTiny.Bantina.Bankinate.MySql.SqlStatementManagement
{
    internal class MySqlCommandTextGenerator : CommandTextGeneratorBase
    {
        public override string Add<TEntity>(SqlDbContext dbContext, TEntity entity)
        {
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            dbContext.Parameters = new Dictionary<string, object>();

            StringBuilder builder_front = new StringBuilder(), builder_behind = new StringBuilder();
            builder_front.Append("INSERT INTO ");
            builder_front.Append(dbContext.TableName);
            builder_front.Append(" (");
            builder_behind.Append(" VALUES (");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //Column
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute column)
                {
                    builder_front.Append(column.GetName(propertyInfo.Name));
                    builder_front.Append(",");
                    builder_behind.Append("@");
                    columnName = column.GetName(propertyInfo.Name).Replace("`", "");
                    builder_behind.Append(columnName);
                    builder_behind.Append(",");

                    dbContext.Parameters.AddOrUpdate($"@{columnName}", propertyInfo.GetValue(entity));
                }

                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                    builder_front.Append(")");

                    builder_behind.Remove(builder_behind.Length - 1, 1);
                    builder_behind.Append(")");
                }
            }
            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append(builder_behind.ToString()).ToString();
        }

        public override string Update<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, bool>> filter, TEntity entity)
        {
            dbContext.Parameters = new Dictionary<string, object>();
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));

            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            builder_front.Append(dbContext.TableName);
            builder_front.Append(" ");

            //查询语句中表的别名，例如“t”
            string alias = filter.Parameters[0].Name;
            builder_front.Append(alias);
            builder_front.Append(" SET ");

            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append($"@{alias}");
                    columnName = columnAttr.GetName(propertyInfo.Name).Replace("`", "");
                    builder_front.Append(columnName);
                    builder_front.Append(",");

                    dbContext.Parameters.AddOrUpdate($"@{alias}{columnName}", propertyInfo.GetValue(entity));
                }
                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }

            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
        }

        public override string Update<TEntity>(SqlDbContext dbContext, TEntity entity, out Expression<Func<TEntity, bool>> filter)
        {
            dbContext.Parameters = new Dictionary<string, object>();
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));

            //查找主键以及主键对应的值，如果用该方法更新数据，主键是必须存在的
            //get property which is key
            var keyProperty = propertyInfos.Where(t => t.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute)?.FirstOrDefault();
            if (keyProperty == null)
                throw new TableKeyNotFoundException($"table '{dbContext.TableName}' not found key column");

            //主键的key
            string keyName = keyProperty.Name;
            //主键的value 
            var keyValue = keyProperty.GetValue(entity);

            if (keyProperty.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr1)
                keyName = columnAttr1.GetName(keyProperty.Name);

            //Generate Expression of update via key : t=>t.'Key'== value
            ParameterExpression param = Expression.Parameter(typeof(TEntity), "t");
            MemberExpression left = Expression.Property(param, keyProperty);
            ConstantExpression right = Expression.Constant(keyValue);
            BinaryExpression where = Expression.Equal(left, right);
            filter = Expression.Lambda<Func<TEntity, bool>>(where, param);

            //将主键的查询参数加到字典中
            dbContext.Parameters.AddOrUpdate($"@t{keyName}", keyValue);

            //开始构造赋值的sql语句
            StringBuilder builder_front = new StringBuilder();
            builder_front.Append("UPDATE ");
            //Mysql和sqlserver的分别处理
            if (dbContext.DataBaseType == DataBaseType.MySql)
            {
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
            }

            //查询语句中表的别名，例如“t”
            string alias = filter.Parameters[0].Name;

            builder_front.Append(alias);
            builder_front.Append(" SET ");

            string columnName = string.Empty;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //AutoIncrease : if property is auto increase attribute skip this column.
                if (propertyInfo.GetCustomAttribute(typeof(AutoIncreaseAttribute), true) is AutoIncreaseAttribute autoIncreaseAttr)
                {
                }
                //Column :
                else if (propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                {
                    builder_front.Append(columnAttr.GetName(propertyInfo.Name));
                    builder_front.Append("=");
                    builder_front.Append($"@{alias}");
                    columnName = columnAttr.GetName(propertyInfo.Name).Replace("`", "");
                    builder_front.Append(columnName);
                    builder_front.Append(",");

                    dbContext.Parameters.AddOrUpdate($"@{alias}{columnName}", propertyInfo.GetValue(entity));
                }
                //in the end,remove the redundant symbol of ','
                if (propertyInfos.Last() == propertyInfo)
                {
                    builder_front.Remove(builder_front.Length - 1, 1);
                }
            }

            //SqlServer和Mysql的sql语句分别处理
            if (dbContext.DataBaseType == DataBaseType.SqlServer)
            {
                builder_front.Append(" FROM ");
                builder_front.Append(dbContext.TableName);
                builder_front.Append(" ");
                builder_front.Append(alias);
            }

            //Generate SqlStatement
            return dbContext.SqlStatement = builder_front.Append($"{LambdaToSql.ConvertWhere(filter)}").ToString();
        }

        public override string Delete<TEntity>(SqlDbContext dbContext, TEntity entity)
        {
            dbContext.Parameters = new Dictionary<string, object>();
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            PropertyInfo[] propertyInfos = GetPropertiesDicByType(typeof(TEntity));
            //get property which is key
            var property = propertyInfos.Where(t => t.GetCustomAttribute(typeof(KeyAttribute), true) is KeyAttribute)?.FirstOrDefault();
            if (property == null)
                throw new TableKeyNotFoundException($"table '{dbContext.TableName}' not found key column");

            string colunmName = property.Name;
            var value = property.GetValue(entity);

            if (property.GetCustomAttribute(typeof(ColumnAttribute), true) is ColumnAttribute columnAttr)
                colunmName = columnAttr.GetName(property.Name);

            dbContext.Parameters.AddOrUpdate($"@t{colunmName}", value);
            return dbContext.SqlStatement = $"DELETE t FROM {dbContext.TableName} t WHERE t.{colunmName} = @t{colunmName}";
        }

        public override string Delete<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, bool>> filter)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            dbContext.TableName = TableAttribute.GetName(typeof(TEntity));
            dbContext.SqlStatement = $"DELETE {filter.Parameters[0].Name} From {dbContext.TableName} {filter.Parameters[0].Name} {LambdaToSql.ConvertWhere(filter, out parameters)}";
            dbContext.Parameters = parameters;
            return dbContext.SqlStatement;
        }

        public override string QueryableWhere<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, bool>> filter)
        {
            string result = LambdaToSql.ConvertWhere(filter, out IDictionary<string, object> parameters);
            dbContext.Parameters = parameters;
            return result;
        }

        public override string QueryableOrderBy<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, object>> orderBy, bool isDESC)
        {
            if (orderBy == null)
                return string.Empty;

            string desc = isDESC ? "DESC" : "ASC";
            string result = $" ORDER BY {LambdaToSql.ConvertOrderBy(orderBy)} {desc}";

            return result;
        }

        public override List<string> QueryableSelect<TEntity>(SqlDbContext dbContext, Expression<Func<TEntity, object>> columns)
        {
            return LambdaToSql.ConvertColumns<TEntity>(columns);
        }

        public override string QueryableQueryCount<TEntity>(SqlDbContext dbContext, string alias, string where)
        {
            return dbContext.SqlStatement = $"SELECT COUNT(0) FROM {dbContext.TableName} {alias} {where}";
        }

        public override string QueryableQuery<TEntity>(SqlDbContext dbContext, List<string> columns, string alias, string where, string orderBy, string top)
        {
            string queryColumns = (columns == null || !columns.Any()) ? "*" : string.Join(",", columns.Select(t => $"{alias}.{t}"));
            return dbContext.SqlStatement = $"SELECT {queryColumns} FROM {dbContext.TableName} {alias} {where} {orderBy} {top}";
        }

        //目前queryablePaging是最终的结果了
        public override string QueryablePaging<TEntity>(SqlDbContext dbContext, List<string> columns, string alias, string where, string orderBy, int pageIndex, int pageSize)
        {
            string queryColumns = (columns == null || !columns.Any()) ? "*" : string.Join(",", columns.Select(t => $"TTTTTT.{t}").ToArray());
            return dbContext.SqlStatement = $"SELECT {string.Join(",", columns.Select(t => $"{alias}.{t}"))} FROM {dbContext.TableName} {alias} {where} {orderBy} LIMIT {pageIndex * pageSize},{pageSize}";
        }
    }
}