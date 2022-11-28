using Domain.Exports.Excel.Base;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Contact;
using Domain.Querys.Inventory;
using Domain.Services;
using System.Threading.Tasks;

namespace Domain.Exports.Excel
{
    public class InventoryExcel : BaseExcelPage<PageQuery, InventoryQuery>
    {
        private InventoryService InventoryService { get; }

        public InventoryExcel(InventoryService inventoryService, PageQuery pageQuery) : base(pageQuery)
        {
            InventoryService = inventoryService;
        }

        public override string PrefixFileName()
        {
            return "Estoque";
        }

        protected override string Title()
        {
            return "Estoque";
        }

        public override Task<PageData<InventoryQuery>> DataTable(PageQuery pageQuery)
        {
            return InventoryService.FindInventory(pageQuery);
        }
    }
}
