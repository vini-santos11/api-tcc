using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface ITransactionRepository : ICrudRepository<long, AppTransaction>
    {
    }
}
