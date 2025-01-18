using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KantorLibrary.Models
{
    public class Zamowienie
    {
       // [JsonPropertyName("id_zamowienia")]
        public int Id { get; set; }
      //  [JsonPropertyName("data_zamowienia")]
        public DateTime Data { get; set; } //data zamówienia
      //  [JsonPropertyName("id_klienta")]
        public int KlientId { get; set; }
        public Klient Klient { get; set; }
      //  [JsonPropertyName("ilosc")]
        public int Ilosc { get; set; }
      //  [JsonPropertyName("wartosc")]
        public decimal Wartosc { get; set; }
      //  [JsonPropertyName("id_lokalizacji")]
        public int LokalizacjaId { get; set; }
        public Lokalizacja Lokalizacja { get; set; }
      //  [JsonPropertyName("id_kursu")]
        public int KursId { get; set; }
        public Kurs Kurs { get; set; }
     //   [JsonPropertyName("kurs")]
        public decimal CenaKursu { get; set; }
     //   [JsonPropertyName("strona")]
        public char Strona { get; set; } //czy kupno czy sprzedaż
     //   [JsonPropertyName("id_adresu")]
        public int AdresId { get; set; }
        public Adres Adres { get; set; }


    }
}
