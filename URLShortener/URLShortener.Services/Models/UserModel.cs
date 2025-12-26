using URLShortener.Services.Enums;

namespace URLShortener.Services.Models
{
    public class UserModel (int id = 0) : AbstractModel (id)
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Nickname { get; set; } = string.Empty;

        public AccountType AccountType { get; set; } = AccountType.None;
    }
}
