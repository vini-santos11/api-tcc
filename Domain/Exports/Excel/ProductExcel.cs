using Domain.Exports.Excel.Base;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Product;
using Domain.Services;
using System.Threading.Tasks;

namespace Domain.Exports.Excel
{
    public class ProductExcel : BaseExcelPage<PageQuery, ProductQuery>
    {
        private ProductService ProductService { get; }

        public ProductExcel(ProductService productService, PageQuery pageQuery) : base(pageQuery)
        {
            ProductService = productService;
        }

        public override string PrefixFileName()
        {
            return "Produtos";
        }

        protected override string Title()
        {
            return "Produtos";
        }

        public override Task<PageData<ProductQuery>> DataTable(PageQuery pageQuery)
        {
            return ProductService.FindAllProducts(pageQuery);
        }
    }
}
