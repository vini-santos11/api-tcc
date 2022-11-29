using Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.PageQuerys
{
    public class HistoryPageQuery : PageQuery
    {
        public EOperation Operation { get; set; }
    }
}
