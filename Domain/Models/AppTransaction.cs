using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Transaction")]
    public class AppTransaction : CreatedModel
    {
        [Key]
        public long Id { get; set; }
        public long? ContactOriginId { get; set; }
        public long? ContactDestinationId { get; set; }
        public long OperationId { get; set; }
        public long ProductId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Amount { get; set; }
    }
}
