using Domain.Enumerables;

namespace Application.Identity
{
    public class Role
    {
        public const string USER = nameof(ERoles.User);
        public const string ADMIN = nameof(ERoles.Admin);
    }
}
