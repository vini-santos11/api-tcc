using Domain.Interfaces;
using Domain.Page.Base;
using Domain.PageQuerys.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Exports.Excel.Base
{
    public abstract class BaseExcelPage<S, T> : ExcelExport<T>, IExportFile where S : BaseQuery
    {
        private readonly S _pageQuery;

        public BaseExcelPage(S pageQuery, bool incDateFileName = true) : base(incDateFileName)
        {
            _pageQuery = pageQuery;
            _pageQuery.Pagination = EPagination.NO;
        }

        protected S PageQuery()
        {
            return _pageQuery;
        }

        public abstract Task<PageData<T>> DataTable(S pageQuery);

        protected abstract string Title();

        protected async override Task<IEnumerable<T>> DataAsync()
        {
            var pageData = await DataTable(_pageQuery);
            return pageData.Data;
        }
    }
}
