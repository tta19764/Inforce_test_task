using URLShortener.Services.Enums;

namespace URLShortener.Services.Models
{
    public class UserModel (int id = 0) : AbstractModel (id)
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string Nickname { get; set; } = string.Empty;

        public AccountLevel AccountType { get; set; } = AccountLevel.Regular;
    }
}
