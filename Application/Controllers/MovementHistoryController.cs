using Application.Controllers.Base;
using Application.Identity;
using Domain.Enumerables;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys.History;
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
    [Route("movement")]
    public class MovementHistoryController : BaseController
    {
        public MovementHistoryService MovementHistoryService { get; set; }
        public MovementHistoryController(MovementHistoryService movementHistoryService)
        {
            MovementHistoryService = movementHistoryService;
        }

        [HttpGet("operation/history")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PageData<MovementHistoryQuery>>> MovementHistory([FromQuery] HistoryPageQuery query)
        {
            return Ok(await MovementHistoryService.FindMovementHistory(query));
        }
    }
}
