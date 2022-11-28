using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Product")]
    public class AppProduct : CreatedModel
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultMeansurement { get; set; }
        public decimal Price { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageUrl { get; set; }
    }
}
