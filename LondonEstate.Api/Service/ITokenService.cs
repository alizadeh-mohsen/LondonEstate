using Microsoft.AspNetCore.Identity;

namespace LondonEstate.Api.Service
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user);
    }
}
