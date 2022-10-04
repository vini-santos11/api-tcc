using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infra.Repositories.Base;

namespace Infra.Repositories
{
    public class ProductRepository : CrudRepository<long, AppProduct>, IProductRepository
    {
        public ProductRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {

        }
}
}
