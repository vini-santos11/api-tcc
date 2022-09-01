using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_StockMovement")]
    public class App_StockMovement : CreatedModel
    {
        [Key]
        public long Id { get; set; }
        public long TransactionItemOriginId { get; set; }
        public long TransactionItemDestinationId { get; set; }
        public double Amount { get; set; }
    }
}
