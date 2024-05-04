using System.ComponentModel.DataAnnotations;

namespace BilirkisiMvc.ViewModels;

public class ParolaYenilemeModeli
{
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-Posta alanı boş geçilemez.")]
    [EmailAddress(ErrorMessage = "Geçersiz e-posta adresi.")]
    [DataType(DataType.EmailAddress)]
    public string Eposta { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola alanı boş geçilemez.")]
    [DataType(DataType.Password)]
    public string Parola { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parola tekrarı alanı boş geçilemez.")]
    [DataType(DataType.Password)]
    [Compare("Parola", ErrorMessage = "Parolalar eşleşmiyor.")]
    public string ParolaTekrar { get; set; } = string.Empty;
}
