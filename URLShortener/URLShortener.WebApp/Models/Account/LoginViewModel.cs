using System.ComponentModel.DataAnnotations;

namespace URLShortener.WebApp.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is reqired.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = true;
        public Uri ReturnUrl { get; set; } = new Uri("/", UriKind.Relative);
    }
}
