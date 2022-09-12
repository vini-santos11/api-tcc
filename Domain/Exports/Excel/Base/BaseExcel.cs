using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exports.Excel.Base
{
    public abstract class BaseExcel<T> : ExcelExport<T>, IExportFile
    {
        public BaseExcel(bool incDateFileName = true) : base(incDateFileName)
        {
        }

        public abstract IEnumerable<T> Data();

        protected abstract string Title();

        protected override Task<IEnumerable<T>> DataAsync()
        {
            return Task.FromResult(Data());
        }

    }
}
