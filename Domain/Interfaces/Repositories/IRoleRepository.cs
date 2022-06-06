using Domain.Enumerables;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces.Repositories
{
    public interface IRoleRepository : ICrudRepository<long, AppRole>, IRoleStore<ERoles>
    {
        AppRole FindByRole(string role);
    }
}
