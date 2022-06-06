using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces.Repositories
{
    public interface IUserRepository : ICrudRepository<long, AppUser>, IUserStore<AppUser>, IUserEmailStore<AppUser>, IUserPhoneNumberStore<AppUser>,
                     IUserTwoFactorStore<AppUser>, IUserPasswordStore<AppUser>, IUserRoleStore<AppUser>, IUserLockoutStore<AppUser>, IUserSecurityStampStore<AppUser>
    {
    }
}
