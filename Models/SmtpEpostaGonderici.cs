
using System.Net;
using System.Net.Mail;

namespace BilirkisiMvc.Models;

public class SmtpEpostaGonderici : IEpostaGonderici
{
    private string? _sunucu;
    private int _port;
    private bool _sslAktif;
    private string? _kullaniciadi;
    private string? _parola;

    public SmtpEpostaGonderici(string? sunucu, int port, bool sslAktif, string? kullaniciadi, string? parola)
    {
        _sunucu = sunucu;
        _port = port;
        _sslAktif = sslAktif;
        _kullaniciadi = kullaniciadi;
        _parola = parola;
    }
    public Task EpostaGonderAsync(string alicieposta, string konu, string icerik)
    {
        var client = new SmtpClient(_sunucu, _port)
        {
            Credentials = new NetworkCredential(_kullaniciadi, _parola),
            EnableSsl = _sslAktif
        };

        return client.SendMailAsync(new MailMessage(_kullaniciadi ?? "", alicieposta, konu, icerik) {IsBodyHtml = true});
    }
}
