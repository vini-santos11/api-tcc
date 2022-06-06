using Domain.Enumerables.Base;

namespace Domain.Enumerables
{
    public class ERoles : BaseEnumeration
    {
        public static readonly ERoles User = new(1, nameof(User));
        public static readonly ERoles Admin = new(1, nameof(Admin));

        public ERoles(int id, string name) : base(id, name)
    {
    }
}
}
