using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Product")]
    public class App_Product : CreatedModel
    {
        [Key]
        public long Id { get; set; }
        public string Description { get; set; }
        public long MeansurementId { get; set; }
    }
}
