using Application.Controllers.Base;
using Application.Identity;
using Domain.Commands;
using Domain.Commands.Contact;
using Domain.Exports.Excel;
using Domain.Models;
using Domain.Page.Base;
using Domain.PageQuerys;
using Domain.Querys;
using Domain.Querys.Contact;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [Authorize]
    [Roles(Role.ADMIN)]
    [Route("contact")]
    public class ContactController : BaseController
    {
        public ContactService ContactService { get; }
        public ContactController(ContactService contactService)
        {
            ContactService = contactService;
        }

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PageData<ContactQuery>>> FindAllContacts([FromQuery] PageQuery pageQuery)
        {
            return Ok(await ContactService.FindAllContacts(pageQuery));
        }

        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<AppContact> FindContactById([FromRoute] long id)
        {
            return Ok(ContactService.FindContactById(id));
        }

        [HttpGet("{id}/image")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<ImageQuery> GetImageContact([FromRoute] long id)
        {
            return Ok(ContactService.GetImageContact(id));
        }

        [HttpGet("export")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> ExportListContacts([FromQuery] PageQuery pageQuery)
        {
            return await ExportFile(new ContactExcel(ContactService, pageQuery));
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<AppContact> CreateContact([FromBody] ContactCommand command)
        {
            return Ok(ContactService.CreateContact(command));
        }

        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<AppContact> UpdateContact([FromRoute] long id, [FromBody] ContactCommand command)
        {
            return Ok(ContactService.UpdateContact(id, command));
        }

        [HttpPut("{id}/image")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult UpdateImageContact([FromRoute] long id, [FromBody] ImageCommand command)
        {
            ContactService.UpdateImageContact(id, command);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult DeleteContact([FromRoute] long id)
        {
            ContactService.DeleteContact(id);
            return Ok();
        }
    }
}
