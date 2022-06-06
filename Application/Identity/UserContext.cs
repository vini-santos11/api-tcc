using Cross.Cutting.IoC;
using Domain.Enumerables;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Application.Identity
{
    public class UserContext: IUserContext
    {
        private readonly IHttpContextAccessor _accessor;

        public UserContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private string ClaimType(IEnumerable<Claim> claims, string claim)
        {
            return claims.Where(c => c.Type == claim).Select(c => c.Value).FirstOrDefault();
        }

        public long? Id => ClaimType(_accessor.HttpContext.User.Claims, ClaimTypes.NameIdentifier)?.ToNullableInt32();

        public string FirstName => ClaimType(_accessor.HttpContext.User.Claims, ClaimTypes.GivenName);

        public string LastName => ClaimType(_accessor.HttpContext.User.Claims, ClaimTypes.Surname);

        public ERoles Role => ERoles.FindByName<ERoles>(ClaimType(_accessor.HttpContext.User.Claims, ClaimTypes.Role));

    }
}
