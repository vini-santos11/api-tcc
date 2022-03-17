using Dapper;
using Dapper.Contrib.Extensions;
using Domain.Attributes;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infra.Repositories.Base
{
    public abstract class CrudRepository<ID, TEntity> : ReadRepository<ID, TEntity>, ICrudRepository<ID, TEntity> where TEntity : class
    {
        private IUserContext UserContext { get; }

        protected CrudRepository(IUserContext userContext, IDBContext dbContext) : base(dbContext)
        {
            UserContext = userContext;
        }

        private void SetChangedAttribute(TEntity model, Type type)
        {
            var properties = typeof(TEntity).GetProperties().Where(p => p.GetCustomAttributes(type, true).Length > 0);

            var longs = new List<Type> { typeof(int), typeof(Nullable<int>), typeof(long), typeof(Nullable<long>) };
            var dates = new List<Type> { typeof(DateTime), typeof(Nullable<DateTime>) };

            foreach (var property in properties)
            {
                if (longs.Contains(property.PropertyType))
                    property.SetValue(model, UserContext.Id);
                else if (dates.Contains(property.PropertyType))
                    property.SetValue(model, DateTime.Now);
            }
        }

        public void Execute(StringBuilder sql, object param = null)
        {
            DBContext.Connection.Execute(sql.ToString(), param);
        }

        public virtual TEntity Add(TEntity model)
        {
            SetChangedAttribute(model, typeof(CreatedAttribute));
            SetChangedAttribute(model, typeof(UpdatedAttribute));

            DBContext.Connection.Insert(FormatModel(model));

            return model;
        }

        public virtual TEntity Update(TEntity model)
        {
            SetChangedAttribute(model, typeof(UpdatedAttribute));

            DBContext.Connection.Update(FormatModel(model));

            return model;
        }

        public virtual bool Remove(TEntity model)
        {
            return DBContext.Connection.Delete(model);
        }

        public virtual bool Remove(ID id)
        {
            return Remove(Instance(id));
        }
    }
}
