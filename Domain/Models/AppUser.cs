using Dapper.Contrib.Extensions;
using Domain.Models.Base;
using System;

namespace Domain.Models
{
    [Table("db_tcc.App_User")]
    public class AppUser : UpdatedModel
    {
        [ExplicitKey]
        public long Id { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public int FailedAccessCount { get; set; }
        public DateTime? LockedEndDate { get; set; }
        public bool EnableTwoFactor { get; set; }
        public bool PhoneConfirmed { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool ChangePassword { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public bool Blocked { get; set; }
    }
}
