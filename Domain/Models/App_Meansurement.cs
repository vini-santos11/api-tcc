using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Meansurement")]
    public class App_Meansurement : CreatedModel
    {
        [Key]
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Description { get; set; }
        public string Initials { get; set; }
        public string ConversionFactor { get; set; }
        public decimal Price { get; set; }
    }
}
