using Domain.Helpers;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Inventory;
using Infra.Repositories.Base;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class InventoryRepository : CrudRepository<long, AppInventory>, IInventoryRepository
    {
        public InventoryRepository(IUserContext userContext, IDBContext dbContext) : base(userContext, dbContext)
        {

        }

        public AppInventory FindByProduct(long productId)
        {
            var sql = new StringBuilder();
            sql.Append(" Select * ");
            sql.Append("   From App_Inventory inv ");
            sql.Append("  Where inv.ProductId = @ProductId ");

            return QuerySingleOrDefault<AppInventory>(sql, new { productId });
        }

        public Task<PageData<InventoryQuery>> FindInventory(PageQuery pageQuery)
        {
            var sql = new StringBuilder();
            sql.Append(" SELECT pro.Id as ProductId, ");
            sql.Append("        pro.Name, ");
            sql.Append("        inv.Amount, ");
            sql.Append("        inv.UpdatedAt as LastUpdate ");
            sql.Append("   FROM db_tcc.App_Inventory inv ");
            sql.Append("  INNER JOIN db_tcc.App_Product pro on (pro.Id = inv.ProductId) ");
            sql.Append("  WHERE pro.Name like @Query ");

            return PageData<InventoryQuery>(sql, pageQuery, "Name");
        }

        public bool ExistsByProduct(List<long> productId)
        {
            var sql = new StringBuilder();
            sql.Append(" Select Case Count(0) When 0 Then 0 Else 1 End as Founded ");
            sql.Append("   From db_tcc.App_Inventory inv ");
            sql.Append($"  Where inv.ProductId in ({FormatHelper.ListToText(productId)}) ");

            return QuerySingleOrDefault<bool>(sql, new { productId });
        }
    }
}
