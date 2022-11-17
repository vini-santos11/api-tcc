using Application.Controllers.Base;
using Application.Identity;
using Domain.Commands;
using Domain.Commands.Product;
using Domain.Exports.Excel;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys;
using Domain.Querys.Product;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [Authorize]
    [Roles(Role.ADMIN)]
    [Route("product")]
    public class ProductController : BaseController
    {
        public ProductService ProductService { get; }
        public ProductController(ProductService productService)
        {
            ProductService = productService;
        }

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PageData<ProductQuery>>> FindAllProducts([FromQuery] PageQuery pageQuery)
        {
            return Ok(await ProductService.FindAllProducts(pageQuery));
        }

        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<AppProduct> FindProductById([FromRoute] long id)
        {
            return Ok(ProductService.FindProductById(id));
        }

        [HttpGet("{id}/image")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<ImageQuery> GetImageProduct([FromRoute] long id)
        {
            return Ok(ProductService.GetImageProduct(id));
        }

        [HttpGet("export")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> ExportListProducts([FromQuery] PageQuery pageQuery)
        {
            return await ExportFile(new ProductExcel(ProductService, pageQuery));
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<AppProduct> CreateProduct([FromBody] ProductCommand command)
        {
            return Ok(ProductService.CreateProduct(command));
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<AppProduct> UpdateProduct([FromRoute] long id, [FromBody] ProductCommand command)
        {
            return Ok(ProductService.UpdateProduct(id, command));
        }

        [HttpPut("{id}/image")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult UpdateImageProduct([FromRoute] long id, [FromBody] ImageCommand command)
        {
            ProductService.UpdateImageProduct(id, command);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult DeleteProduct([FromRoute] long id)
        {
            ProductService.DeleteProduct(id);
            return Ok();
        }
    }
}
