using Domain.Enumerables;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Repositories.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class RoleRepository : CrudRepository<long, AppRole>, IRoleRepository
    {
        public RoleRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {
        }

        private static AppRole Convert(ERoles role)
        {
            return new AppRole
            {
                Id = role.Id,
                Role = role.Name
            };
        }

        private static ERoles Convert(AppRole role)
        {
            return ERoles.FindById<ERoles>(role.Id);
        }

        public AppRole FindByRole(string role)
        {
            var sql = new StringBuilder();
            sql.Append(" Select ID ");
            sql.Append("   From App_Role ");
            sql.Append("  Where Upper(Role) = Upper(@Role)");

            return QuerySingleOrDefault<AppRole>(sql, new { Role = role?.ToUpperInvariant() });
        }

        public Task<IdentityResult> CreateAsync(ERoles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Add(Convert(role));

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ERoles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Update(Convert(role));

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ERoles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Remove(Convert(role));

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ERoles> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (int.TryParse(roleId, out var id))
                return Task.FromResult(Convert(Find(id)));

            throw new ValidateException(Messages.InvalidProfile);
        }

        public Task<ERoles> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(Convert(FindByRole(normalizedRoleName)));
        }

        public Task<string> GetNormalizedRoleNameAsync(ERoles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.ToString());
        }

        public Task<string> GetRoleIdAsync(ERoles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(ERoles role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.ToString());
        }

        public Task SetNormalizedRoleNameAsync(ERoles role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(ERoles role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Nothing to dispose.
            GC.SuppressFinalize(this);
        }
    }
}
