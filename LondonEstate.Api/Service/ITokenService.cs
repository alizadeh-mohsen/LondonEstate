using Microsoft.AspNetCore.Identity;

namespace LondonEstate.Api.Service
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user);
        string CreateRefreshToken(IdentityUser user);
        string? ValidateRefreshToken(string refreshToken);
        Task<bool> SaveRefreshTokenAsync(string userId, string tokenHash, DateTime expiry);
        Task<bool> RevokeRefreshTokenAsync(string tokenHash);
    }
}
