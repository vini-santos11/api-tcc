using Application.Controllers.Base;
using Domain.Commands.User;
using Domain.Querys;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Application.Controllers
{
    [Route("auth")]
    public class AuthenticationController : BaseController
    {
        private AuthenticationService AuthenticationService { get; }
        public AuthenticationController(AuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        /// <summary>
        /// Register new user with role USER
        /// </summary>
        [HttpPost("sign-up")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserTokenQuery), 201)]
        public async Task<ActionResult<UserTokenQuery>> Register([FromBody] RegisterCommand command)
        {
            return Ok(await AuthenticationService.RegisterUser(command));
        }

        /// <summary>
        /// Authenticate user in application
        /// </summary>
        [HttpPost("sign-in")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserTokenQuery), 200)]
        [ProducesResponseType(typeof(IdentityError[]), 400)]
        [ProducesResponseType(typeof(IdentityError), 401)]
        public async Task<ActionResult<UserTokenQuery>> Authenticate([FromBody] AccessCommand command)
        {
            return Ok(await AuthenticationService.AuthenticateUser(command));
        }

        /// <summary>
        /// Generate access token by refresh token
        /// </summary>
        [HttpPost("refresh")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(UserTokenQuery), 200)]
        [ProducesResponseType(typeof(IdentityError[]), 400)]
        [ProducesResponseType(typeof(IdentityError), 401)]
        public async Task<ActionResult<UserTokenQuery>> Refresh([FromBody] RefreshCommand command)
        {
            return Ok(await AuthenticationService.AuthenticateRefreshToken(command));
        }
    }
}
