using Dapper.Contrib.Extensions;

namespace Domain.Models
{
    [Table("db_tcc.App_UserRole")]
    public class AppUserRole
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
}
