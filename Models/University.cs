namespace BilirkisiMvc.Models;
public class University
{
    public int BirimId { get; set; }
    public string BirimAdi { get; set; }
    public string BirimIngilizceAdi { get; set; }
    public UniversityType UniversiteTuru { get; set; }
    public UniversityActivity Aktiflik { get; set; }
    public City Il { get; set; }
    public District Ilce { get; set; }
    public bool Buyuksehir { get; set; }
    public bool DenizKiyisi { get; set; }
    public int DegisenBirimId { get; set; }
}
public class UniversityType
{
    public int Kod { get; set; }
    public string Ad { get; set; }
}

public class UniversityActivity
{
    public int Kod { get; set; }
    public string Ad { get; set; }
}

public class City
{
    public int Kod { get; set; }
    public string Ad { get; set; }
}

public class District
{
    public int Kod { get; set; }
    public string Ad { get; set; }
}