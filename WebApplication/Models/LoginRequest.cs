namespace WebApplication.Models
{
    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}