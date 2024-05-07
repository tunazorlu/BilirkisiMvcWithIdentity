using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilirkisiMvc.Controllers;

[Authorize(Roles = "Admin")]
public class SoapController : Controller
{
    public async Task<ActionResult> Listele()
    {
        try
        {
            string url = "https://servisler.yok.gov.tr/ws/g2g/BRM-REF-000-7E6A2961";
            string soapXml = @"
                <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://www.yok.gov.tr/yuksekogretimegitimbilgisi/2019/10"">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <ns:BirimUniversiteRequest>
                            <ns:Istek>
                                <ns:IstekTarihi>?</ns:IstekTarihi>
                                <ns:IstekId>?</ns:IstekId>
                                <ns:KullaniciAdi>?</ns:KullaniciAdi>
                            </ns:Istek>
                        </ns:BirimUniversiteRequest>
                    </soapenv:Body>
                </soapenv:Envelope>";

            using (var httpClient = new HttpClient())
            {
                // Basic Authentication için username ve password ekleyin
                var username = "980117";
                var password = "d3fN%p6q#kM3";
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(soapXml, Encoding.UTF8, "text/xml");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                string responseXml = await response.Content.ReadAsStringAsync();

                // XML isim alanı tanımlaması
                XNamespace ns = "http://www.yok.gov.tr/yuksekogretimegitimbilgisi/2019/10";
                XDocument xmlDoc = XDocument.Parse(responseXml);

                // Universite düğümlerini seçme
                var universityNodes = xmlDoc.Descendants(ns + "Universite");

                // Model olarak XDocument'i view'e geçirin
                return View(universityNodes);
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }


    // SOAP yanıtını işlemek ve hata kontrolü yapmak için kullanılacak metot
    private void HandleSoapResponse(XDocument xmlDoc)
    {
        // Eğer XML belgesi null ise veya içinde beklenen veri yoksa, uygun bir hata mesajı gösterin veya loglayın
        if (xmlDoc.Root == null || !xmlDoc.Descendants().Any())
        {
            throw new Exception("SOAP yanıtında beklenen veri bulunamadı veya yanıt boş.");
        }
        // SOAP yanıtının doğruluğunu ve beklenen yapısını kontrol etmek için gerekli diğer işlemleri burada yapın
    }

}
