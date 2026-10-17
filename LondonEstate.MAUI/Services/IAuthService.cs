namespace LondonEstate.MAUI.Services
{
    /// <summary>
    /// Authentication service interface for handling login, logout, and token management
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Login with email and password, stores token in secure storage
        /// </summary>
        Task<string> LoginAsync(string email, string password);

        /// <summary>
        /// Logout and remove token from secure storage
        /// </summary>
        Task LogoutAsync();

        /// <summary>
        /// Get the stored access token from secure storage
        /// </summary>
        Task<string?> GetTokenAsync();

        /// <summary>
        /// Check if user is authenticated (token exists)
        /// </summary>
        Task<bool> IsAuthenticatedAsync();

        /// <summary>
        /// Refresh the access token using refresh token
        /// </summary>
        Task<string> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Clear all stored authentication data
        /// </summary>
        Task ClearAuthenticationAsync();
    }
}
