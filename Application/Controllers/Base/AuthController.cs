using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Application.Controllers.Base
{
    [Authorize]
    public abstract class AuthController : BaseController
    {
        protected IUserContext UserContext { get; }
        protected AuthController(IUserContext userContext)
        {
            UserContext = userContext;
        }
    }
}
