using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Repositories.Base;

namespace Infra.Repositories
{
    public class UserRoleRepository : CrudRepository<long, AppUserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {
        }
    }
}
