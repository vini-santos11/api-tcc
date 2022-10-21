using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infra.Repositories
{
    public class UserRoleRepository : CrudRepository<long, AppUserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {
        }

        public IEnumerable<AppUserRole> FindUserRoleByUserId(long userId)
        {
            var sql = new StringBuilder();
            sql.Append(" Select * ");
            sql.Append("   From db_tcc.App_UserRole ur ");
            sql.Append("  Where ur.UserId = @UserId");

            return QueryToList<AppUserRole>(sql, new { userId });
        }
    }
}
