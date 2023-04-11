using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Application.Controllers.Base
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected BaseController()
        {
        }

        protected async Task<ActionResult> ExportFile(IExportFile exportFile)
        {
            using var memory = exportFile.Export();

            var file = await memory;
            if ((file == null) || (file.Length == 0))
                return NoContent();

            return File(file, exportFile.ContentType, exportFile.FileName);
        }
    }
}
