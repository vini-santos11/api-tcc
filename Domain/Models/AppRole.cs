using Dapper.Contrib.Extensions;

namespace Domain.Models
{
    [Table("db_tcc.App_Role")]
    public class AppRole
    {
        [ExplicitKey]
        public int Id { get; set; }
        public string Role { get; set; }
    }
}
