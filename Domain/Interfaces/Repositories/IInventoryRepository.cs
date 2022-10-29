using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Inventory;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
    public interface IInventoryRepository : ICrudRepository<long, AppInventory>
    {
        AppInventory FindByProduct(long productId);
        Task<PageData<InventoryQuery>> FindInventory(PageQuery pageQuery);
    }
}
