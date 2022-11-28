using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_TransactionItem")]
    public class AppTransactionItem : CreatedModel
    {
        [Key]
        public long Id { get; set; }
        public long ProductId { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public double TotalPrice { get; set; }
    }
}
