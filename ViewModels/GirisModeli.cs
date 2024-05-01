using System.ComponentModel.DataAnnotations;

namespace BilirkisiMvc.ViewModels
{
    public class GirisModeli
    {
        [Required]
        [EmailAddress]
        public string Eposta { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string? Parola { get; set; } = null!;
        public bool BeniHatirla { get; set; } = true;
    }
}