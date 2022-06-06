using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Application.Identity
{
    public class ResetPasswordTokenProvider : TotpSecurityStampBasedTokenProvider<AppUser>
    {
        public const string ProviderKey = "ResetPassword";

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
        {
            return Task.FromResult(false);
        }
    }
}
