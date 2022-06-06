using CrossCutting.Configurations;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class UserRepository : CrudRepository<long, AppUser>, IUserRepository
    {
        private PasswordConfiguration PasswordConfiguration { get; }
        public UserRepository(
            IDBContext dbContext,
            IUserContext userContext,
            IOptions<PasswordConfiguration> passwordConfiguration) : base(userContext, dbContext)
        {
            PasswordConfiguration = passwordConfiguration.Value;
        }

        private string FindDocumentById(long id)
        {
            var sql = new StringBuilder();
            sql.Append(" Select DocumentNumber ");
            sql.Append("   From App_Contact");
            sql.Append("  Where Id = @Id");

            return QueryFirstOrDefault<string>(sql, new { id });
        }

        private AppUser FindByDocument(string login)
        {
            var sql = new StringBuilder();
            sql.Append(" Select usr.* ");
            sql.Append("   From App_Contact con ");
            sql.Append("  Inner Join App_User usr on (usr.Id = con.Id) ");
            sql.Append("  Where (con.DocumentNumber = @Document Or ");
            sql.Append("         usr.RefreshToken = @Login Or ");
            sql.Append("         con.Email = @Login) ");

            return QueryFirstOrDefault<AppUser>(sql, new { login, Document = FormatHelper.Document(login) });
        }

        private string FindEmailContactByUserId(long id)
        {
            var sql = new StringBuilder();
            sql.Append(" Select con.Email ");
            sql.Append("   From App_Contact con ");
            sql.Append("  Where con.Id = @Id");

            return QueryFirstOrDefault<string>(sql, new { id });
        }

        private string FindPhoneContactByUserId(long id)
        {
            var sql = new StringBuilder();
            sql.Append(" Select con.Phone ");
            sql.Append("   From App_Contact con ");
            sql.Append("  Where con.Id = @Id");

            return QueryFirstOrDefault<string>(sql, new { id });
        }

        private IList<string> FindRolesByUserId(long id)
        {
            var sql = new StringBuilder();
            sql.Append(" Select Upper(rol.Role) As Role ");
            sql.Append("   From App_UserRole uro ");
            sql.Append("  Inner Join App_Role rol on (rol.Id = uro.RoleId) ");
            sql.Append("  Where uro.UserId = @Id");

            return QueryToList<string>(sql, new { id }).ToList();
        }

        private bool IsInRoleByUserIdAndRoleName(long userId, string roleName)
        {
            var sql = new StringBuilder();
            sql.Append(" Select Case When Count(0) = 0 Then 0 Else 1 End As Found ");
            sql.Append("   From App_UserRole uro ");
            sql.Append("  Inner Join App_Role rol on (rol.Id = uro.RoleId) ");
            sql.Append("  Where uro.UserId = @UserId");
            sql.Append("    And Upper(rol.Role) = @RoleName");

            return QueryFirstOrDefault<bool>(sql, new { userId, RoleName = roleName.ToUpperInvariant() });
        }

        private IList<AppUser> FindByRoleName(string roleName)
        {
            var sql = new StringBuilder();
            sql.Append(" Select usr.* ");
            sql.Append("   From App_User usr ");
            sql.Append("  Inner Join App_UserRole uro on (uro.UserID = usr.Id) ");
            sql.Append("  Inner Join App_Role rol on (rol.Id = uro.RoleId) ");
            sql.Append("  Where Upper(rol.Role) = @RoleName");

            return QueryToList<AppUser>(sql, new { RoleName = roleName.ToUpperInvariant() }).ToList();
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (int.TryParse(userId, out var id))
                return Task.FromResult(Find(id));

            return Task.FromResult(new AppUser());
        }

        public Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindByDocument(normalizedUserName));
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindDocumentById(user.Id));
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindDocumentById(user.Id));
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindEmailContactByUserId(user.Id));
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.EmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return FindByNameAsync(normalizedEmail, cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindEmailContactByUserId(user.Id));
        }

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(AppUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task<string> GetPhoneNumberAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindPhoneContactByUserId(user.Id));
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PhoneConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PhoneConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.EnableTwoFactor = enabled;
            return Task.CompletedTask;
        }

        public Task<bool> GetTwoFactorEnabledAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.EnableTwoFactor);
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.Password = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        public Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindRolesByUserId(user.Id));
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(IsInRoleByUserIdAndRoleName(user.Id, roleName));
        }

        public Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(FindByRoleName(roleName));
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user = Find(user.Id);

            return Task.FromResult(user.LockedEndDate.HasValue ? new DateTimeOffset(user.LockedEndDate.Value) : (DateTimeOffset?)null);
        }

        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user = Find(user.Id);
            user.LockedEndDate = lockoutEnd.HasValue ? lockoutEnd.Value.LocalDateTime : (DateTime?)null;
            Update(user);

            return Task.CompletedTask;
        }

        public Task<int> IncrementAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user = Find(user.Id);
            user.FailedAccessCount++;

            Update(user);

            return Task.FromResult(user.FailedAccessCount);
        }

        public Task ResetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user = Find(user.Id);
            user.FailedAccessCount = 0;
            Update(user);

            return Task.CompletedTask;
        }

        public Task<int> GetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user = Find(user.Id);

            return Task.FromResult(user.FailedAccessCount);
        }

        public Task<bool> GetLockoutEnabledAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(PasswordConfiguration.UserLockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(enabled);
        }

        public Task SetSecurityStampAsync(AppUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Id.ToString());
        }

        public void Dispose()
        {
            // Nothing to dispose.
            GC.SuppressFinalize(this);
        }

        public Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Add(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Update(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Remove(user);

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
