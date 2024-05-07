using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BilirkisiMvc.Models;

namespace BilirkisiMvc.Servisler
{
    public class SoapServiceClient
    {
        private readonly HttpClient _client;

        public SoapServiceClient()
        {
            _client = new HttpClient();
        }

        public async Task<IEnumerable<University>> GetUniversitiesAsync(string username, string password)
        {
            // SOAP isteği oluşturma
            string soapEnvelope = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                    <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://www.yok.gov.tr/yuksekogretimegitimbilgisi/2019/10"">
                                        <soapenv:Header/>
                                        <soapenv:Body>
                                            <ns:BirimUniversiteRequest>
                                                <ns:Istek>
                                                    <ns:IstekTarihi>{0}</ns:IstekTarihi>
                                                    <ns:IstekId>{1}</ns:IstekId>
                                                    <ns:KullaniciAdi>{2}</ns:KullaniciAdi>
                                                    <ns:UygulamaAdi>{3}</ns:UygulamaAdi>
                                                    <ns:UygulamaSunucuAdi>{4}</ns:UygulamaSunucuAdi>
                                                </ns:Istek>
                                            </ns:BirimUniversiteRequest>
                                        </soapenv:Body>
                                    </soapenv:Envelope>";

            string soapRequest = string.Format(soapEnvelope, DateTime.Now, Guid.NewGuid(), username, "UygulamaAdi", "UygulamaSunucuAdi");

            // HTTP isteği yapılandırma
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://servisler.yok.gov.tr/ws/g2g/BRM-REF-000-7E6A2961"),
                Method = HttpMethod.Post,
                Content = new StringContent(soapRequest, Encoding.UTF8, "text/xml")
            };

            // Basic Auth eklemek
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            // SOAP isteğini gönderme
            var response = await _client.SendAsync(request);

            // Yanıtı alma
            string soapResponse = await response.Content.ReadAsStringAsync();

            // SOAP yanıtını C# nesnelerine dönüştürme
            var universities = SoapResponseToUniversities(soapResponse);

            return universities;
        }

        private IEnumerable<University> SoapResponseToUniversities(string soapResponse)
        {
            var universities = new List<University>();

            // SOAP yanıtını XML olarak işleme
            XDocument doc = XDocument.Parse(soapResponse);
            XNamespace ns = "http://www.yok.gov.tr/yuksekogretimegitimbilgisi/2019/10";

            // XML'den universiteleri çıkarma
            foreach (var element in doc.Descendants(ns + "Universite"))
            {
            var university = new University
            {
                BirimId = (int)element.Element(ns + "BirimId"),
                BirimAdi = (string)element.Element(ns + "BirimAdi"),
                BirimIngilizceAdi = (string)element.Element(ns + "BirimIngilizceAdi"),
                UniversiteTuru = new UniversityType
                {
                Kod = (int)element.Element(ns + "UniversiteTuru").Element(ns + "Kod"),
                Ad = (string)element.Element(ns + "UniversiteTuru").Element(ns + "Ad")
                },
                Aktiflik = new UniversityActivity
                {
                Kod = (int)element.Element(ns + "Aktiflik").Element(ns + "Kod"),
                Ad = (string)element.Element(ns + "Aktiflik").Element(ns + "Ad")
                },
                Il = new City
                {
                Kod = (int)element.Element(ns + "Il").Element(ns + "Kod"),
                Ad = (string)element.Element(ns + "Il").Element(ns + "Ad")
                },
                Ilce = new District
                {
                Kod = (int)element.Element(ns + "Ilce").Element(ns + "Kod"),
                Ad = (string)element.Element(ns + "Ilce").Element(ns + "Ad")
                },
                Buyuksehir = element.Element(ns + "Buyuksehir")?.Element(ns + "Ad")?.Value == "Evet",
                DenizKiyisi = element.Element(ns + "DenizKiyisi")?.Element(ns + "Ad")?.Value == "Evet",
                DegisenBirimId = (int)element.Element(ns + "DegisenBirimId")
            };

            universities.Add(university);
            }

            return universities;
        }
    }
}
