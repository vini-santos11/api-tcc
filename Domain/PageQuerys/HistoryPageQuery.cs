using Domain.Enumerables;

namespace Domain.PageQuerys
{
    public class HistoryPageQuery : PageQuery
    {
        public long? ContactId { get; set; }
        public EOperation Operation { get; set; }
    }
}
