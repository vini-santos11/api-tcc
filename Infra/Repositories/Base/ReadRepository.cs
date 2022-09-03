using Dapper;
using Domain.Interfaces;
using Domain.Page.Base;
using Domain.PageQuerys.Base;
using Infra.Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories.Base
{
    public abstract class ReadRepository<ID, TEntity>: IReadRepository<ID, TEntity> where TEntity : class
    {
        protected IDBContext DBContext { get; }

        protected ReadRepository(IDBContext dbContext)
        {
            DBContext = dbContext;
        }

        protected static TEntity Instance(ID id)
        {
            var key = DapperHelper<TEntity>.FieldKey().FirstOrDefault();
            var model = (TEntity)Activator.CreateInstance(typeof(TEntity));
            key.SetValue(model, id);

            return model;
        }

        public IEnumerable<TQuery> Query<TQuery>(StringBuilder sql, object param = null)
        {
            return DBContext.Connection.Query<TQuery>(sql.ToString(), param);
        }

        public TQuery QuerySingleOrDefault<TQuery>(StringBuilder sql, object param = null)
        {
            return DBContext.Connection.QuerySingleOrDefault<TQuery>(sql.ToString(), param);
        }

        public TQuery QueryFirstOrDefault<TQuery>(StringBuilder sql, object param = null)
        {
            return DBContext.Connection.QueryFirstOrDefault<TQuery>(sql.ToString(), param);
        }

        public IEnumerable<TQuery> QueryToList<TQuery>(StringBuilder sql, object param = null, IDbTransaction transaction = null, int timeout = 600)
        {
            var result = DBContext.Connection.Query<TQuery>(sql.ToString(), param, transaction, true, timeout);
            return result;
        }

        public Task<PageData<TQuery>> PageData<TQuery>(StringBuilder sql, BaseQuery pageQuery, string sortColumn, object param = null, IDbTransaction transaction = null)
        {
            var pageData = new PageData<TQuery>
            {
                Data = new List<TQuery>()
            };

            param ??= pageQuery;

            if (pageQuery.Execution == EExecution.PAGE)
            {
                var temporarySql = DapperHelper<TEntity>.PaginatedFilter(sql, pageQuery, sortColumn);
                pageData.Data = DBContext.Connection.Query<TQuery>(temporarySql.ToString(), param, transaction, true, 600);
            }
            if ((pageQuery.Size > 0) && ((pageQuery.Pagination == EPagination.YES) || (pageQuery.Execution == EExecution.COUNT)))
            {
                var temporarySql = DapperHelper<TEntity>.CountPageData(sql);
                pageData.Total = DBContext.Connection.QuerySingle<int>(temporarySql.ToString(), param, transaction, 600);
            }
            else
                pageData.Total = pageData.Data.Count();

            pageData.Page = pageQuery.Page;
            pageData.Size = pageQuery.Size;

            return Task.FromResult(pageData);
        }

        public virtual TEntity FormatModel(TEntity model)
        {
            return model;
        }

        public virtual TEntity Find(TEntity model)
        {
            #region [ Sql ]
            var sql = new StringBuilder();
            sql.Append($" Select * ");
            sql.Append($"   From {DapperHelper<TEntity>.TableName()} ");
            sql = DapperHelper<TEntity>.Condition(sql);
            #endregion

            return DBContext.Connection.QuerySingleOrDefault<TEntity>(sql.ToString(), FormatModel(model)); ;
        }

        public virtual TEntity Find(ID id)
        {
            return Find(Instance(id));
        }

        public virtual bool Exists(TEntity model)
        {
            #region [ Sql ]
            var sql = new StringBuilder();
            sql.Append($" Select Case Count(0) When 0 Then 0 Else 1 End as Found ");
            sql.Append($"   From {DapperHelper<TEntity>.TableName()}");
            sql = DapperHelper<TEntity>.Condition(sql);
            #endregion

            return DBContext.Connection.QuerySingleOrDefault<bool>(sql.ToString(), FormatModel(model));
        }

        public virtual bool Exists(ID id)
        {
            return Exists(Instance(id));
        }

    }
}
