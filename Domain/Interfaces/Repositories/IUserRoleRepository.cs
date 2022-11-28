using Domain.Models;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRoleRepository : ICrudRepository<long, AppUserRole>
    {
        IEnumerable<AppUserRole> FindUserRoleByUserId(long userId);
    }
}
