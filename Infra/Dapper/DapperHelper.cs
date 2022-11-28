using Dapper.Contrib.Extensions;
using Domain.PageQuerys.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Dapper
{
    public static class DapperHelper<TEntity>
    {
        public static string TableName()
        {
            return typeof(TEntity).GetCustomAttribute<TableAttribute>(false)?.Name ?? throw new ArgumentException("Entity must have one [Table] property");
        }

        public static IEnumerable<PropertyInfo> FieldKey()
        {
            var key = typeof(TEntity).GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0);
            if ((key == null) || (!key.Any()))
                key = typeof(TEntity).GetProperties().Where(p => p.GetCustomAttributes(typeof(ExplicitKeyAttribute), true).Length > 0);

            return key;
        }

        public static StringBuilder Condition(StringBuilder sql)
        {
            var operador = " Where";

            var keys = FieldKey();
            foreach (var key in keys)
            {
                sql.Append($"{operador} {key.Name} = @{key.Name}");
                operador = "   And";
            }

            return sql;
        }

        public static StringBuilder PaginatedFilter(StringBuilder sqlBuilder, BaseQuery pageQuery, string sortColumn)
        {
            var offset = pageQuery.Size * (pageQuery.Page - 1);
            var sql = new StringBuilder();
            sql.Append(" Select * ");
            sql.Append($"  From ( {sqlBuilder} ) tab");

            if (!string.IsNullOrEmpty(pageQuery.Order))
                sql.Append($"  Order By {pageQuery.Order.ToUpper()} {pageQuery.Sort} ");
            else if (!string.IsNullOrEmpty(sortColumn))
                sql.Append($"  Order By {sortColumn} ");

            if ((pageQuery.Pagination == EPagination.YES) && (pageQuery.Size > 0))
                sql.Append($"   Limit {pageQuery.Size} Offset {offset}");

            return sql;
        }

        public static StringBuilder CountPageData(StringBuilder sqlBuilder)
        {
            var sql = new StringBuilder();
            sql.Append(" Select Count(1) ");
            sql.Append("   From ( ");
            sql.Append(sqlBuilder.ToString());
            sql.Append("        ) tab ");

            return sql;
        }
    }
}
