using Dapper.Contrib.Extensions;
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
    }
}
