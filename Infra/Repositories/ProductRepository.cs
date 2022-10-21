using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Product;
using Infra.Repositories.Base;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class ProductRepository : CrudRepository<long, AppProduct>, IProductRepository
    {
        public ProductRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {

        }

        public Task<PageData<ProductQuery>> FindAllProducts(PageQuery pageQuery)
        {
            var sql = new StringBuilder();
            sql.Append(" Select * ");
            sql.Append("   From App_Product pro "); 
            sql.Append("  Where (pro.Description Like @Query Or ");
            sql.Append("         pro.DefaultMeansurement Like @Query Or ");
            sql.Append("         pro.Price Like @Query) ");

            return PageData<ProductQuery>(sql, pageQuery, "Description");
        }
    }
}
