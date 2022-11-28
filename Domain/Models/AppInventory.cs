using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Inventory")]
    public class AppInventory : UpdatedModel
    {
        [Key]

        public long Id { get; set; }
        public long ProductId { get; set; }
        public decimal Amount { get; set; }
    }
}
