using System.ComponentModel.DataAnnotations;

namespace BilirkisiMvc.ViewModels
{
    public class KullaniciOlusturmaModeli
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Girilen parolalar uyuşmuyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
    }
}