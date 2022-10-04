using Dapper.Contrib.Extensions;
using Domain.Models.Base;

namespace Domain.Models
{
    [Table("db_tcc.App_Operation")]
    public class AppOperation : CreatedModel
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
