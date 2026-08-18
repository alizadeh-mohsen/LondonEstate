namespace LondonEstate.MAUI.Dtos
{
    public class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? Message { get; set; }
    }
}
