
using System.Net.Mail;

namespace BilirkisiMvc.Models;

public class SmtpEpostaGonderici : IEpostaGonderici
{
    private string? _sunucu;
    private int _port;
    private string? _kullaniciadi;
    private string? _parola;
    private bool _sslAktif;
    public SmtpEpostaGonderici(string? sunucu, int port, bool sslAktif, string? kullaniciadi, string? parola)
    {
        _sunucu = sunucu;
        _port = port;
        _kullaniciadi = kullaniciadi;
        _parola = parola;
        _sslAktif = sslAktif;
    }
    public Task EpostaGonderAsync(string alicieposta, string konu, string icerik)
    {
        var client = new SmtpClient(_sunucu, _port)
        {
            Credentials = new System.Net.NetworkCredential(_kullaniciadi, _parola),
            EnableSsl = _sslAktif
        };

        return client.SendMailAsync(new MailMessage(_kullaniciadi ?? "", alicieposta, konu, icerik) {IsBodyHtml = true});
    }
}
