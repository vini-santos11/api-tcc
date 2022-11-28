using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys;
using Domain.Querys.Product;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IProductRepository : ICrudRepository<long, AppProduct>
    {
        Task<PageData<ProductQuery>> FindAllProducts(PageQuery pageQuery);
        ImageQuery GetImageProduct(long productId);
    }
}
