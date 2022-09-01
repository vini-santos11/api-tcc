using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Inventory")]
    public class App_Inventory : CreatedModel
    {
        [Key]

        public long Id { get; set; }
        public long ProductId { get; set; }
        public double Amount { get; set; }
    }
}
