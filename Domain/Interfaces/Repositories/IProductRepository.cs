using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IProductRepository : ICrudRepository<long, AppProduct>
    {
    }
}
