using System.ComponentModel.DataAnnotations;

namespace BilirkisiMvc.ViewModels
{
    public class KullaniciDuzenlemeModeli
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Girilen parolalar uyuşmuyor.")]
        public string? ConfirmPassword { get; set; }

        public IList<string>? SeciliRol { get; set; }
    }
}