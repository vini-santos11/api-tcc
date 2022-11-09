using Application.Controllers.Base;
using Application.Identity;
using Domain.Querys.History;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;

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

        [HttpGet("purchase/{contactId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<IEnumerable<MovementHistoryQuery>> FindCustomerPurchase([FromRoute] long contactId)
        {
            return Ok(MovementHistoryService.FindCustomerPurchase(contactId));
        }
    }
}
