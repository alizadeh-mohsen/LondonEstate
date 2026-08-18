namespace LondonEstate.MAUI.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task LogoutAsync();
    }
}