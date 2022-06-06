using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRoleRepository : ICrudRepository<long, AppUserRole>
    {
    }
}
