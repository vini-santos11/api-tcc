namespace CrossCutting.Configurations
{
    public class PasswordConfiguration
    {
        public bool RequireDigit { get; set; }
        public int RequiredLength { get; set; }
        public bool RequireNonAlphanumeric { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireLowercase { get; set; }
        public bool RequireConfirmedEmail { get; set; }
        public bool UserLockoutEnabled { get; set; }
        public double LockoutTimeSpan { get; set; }
        public int LockoutMaxFailedAccess { get; set; }
    }
}
