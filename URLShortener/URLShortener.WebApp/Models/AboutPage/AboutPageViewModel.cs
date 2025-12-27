using System.ComponentModel.DataAnnotations;

namespace URLShortener.WebApp.Models.AboutPage
{
    public class AboutPageViewModel
    {
        [Required(ErrorMessage = "Content is reqired.")]
        [Display(Name = "Algorythm description")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Created at")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Modified at")]
        public DateTime LastModified { get; set; }

        [Display(Name = "Modified by")]
        public string? LastModifiedBy { get; set; }
    }
}
