using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CrossCutting.Configurations
{
    public class TokenConfiguration
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
        public string Key { get; set; }

        public SecurityKey AccessKey
        {
            get => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        }

        public SigningCredentials SigningCredentials
        {
            get => new(AccessKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
