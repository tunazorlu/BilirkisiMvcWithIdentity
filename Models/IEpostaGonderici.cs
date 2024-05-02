namespace BilirkisiMvc.Models;

public interface IEpostaGonderici
{
    Task EpostaGonderAsync(string alicieposta, string konu, string icerik);
}
