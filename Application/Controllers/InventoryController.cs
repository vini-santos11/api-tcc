using Application.Controllers.Base;
using Application.Identity;
using Domain.Commands;
using Domain.Commands.Inventory;
using Domain.Exports.Excel;
using Domain.Interfaces;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.Inventory;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [Authorize]
    [Roles(Role.ADMIN)]
    [Route("inventory")]
    public class InventoryController : AuthController
    {
        public InventoryService InventoryService { get; set; }
        public InventoryController(
            IUserContext userContext,
            InventoryService inventoryService) : base(userContext)
        {
            InventoryService = inventoryService;
        }

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PageData<InventoryQuery>>> FindInventory([FromQuery] PageQuery pageQuery)
        {
            return Ok(await InventoryService.FindInventory(pageQuery));
        }

        [HttpGet("export")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> ExportInventory([FromQuery] PageQuery pageQuery)
        {
            return await ExportFile(new InventoryExcel(InventoryService, pageQuery));
        }

        [HttpPost("movement")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<AppInventory> InventoryMovement([FromBody] ProductInventoryCommand command)
        {
            InventoryService.InventoryMovement(command, UserContext.Id.GetValueOrDefault(0));
            return Ok();
        }
    }
}
