using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys;
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
            sql.Append(" Select pro.*, ");
            sql.Append("        inv.Amount ");
            sql.Append("   From App_Product pro ");
            sql.Append("   Left Join App_Inventory inv on (inv.ProductId = pro.Id) ");
            sql.Append("  Where (pro.Description Like @Querys Or ");
            sql.Append("        pro.DefaultMeansurement Like @Querys Or ");
            sql.Append("        pro.Price Like @Querys) ");

            return PageData<ProductQuery>(sql, pageQuery, "Description");
        }

        public ImageQuery GetImageProduct(long productId)
        {
            var sql = new StringBuilder();
            sql.Append(" Select pro.Id, ");
            sql.Append("        pro.ImageName, ");
            sql.Append("        pro.ImageUrl ");
            sql.Append("   From App_Product pro ");
            sql.Append("  Where pro.Id = @ProductId ");

            return QuerySingleOrDefault<ImageQuery>(sql, new { productId });
        }
    }
}
