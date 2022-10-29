using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Repositories.Base;

namespace Infra.Repositories
{
    public class TransactionRepository : CrudRepository<long, AppTransaction>, ITransactionRepository
    {
        public TransactionRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {

        }
    }
}
