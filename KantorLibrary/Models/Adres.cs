using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KantorLibrary.Models
{
    public class Adres
    {
        public int Id { get; set; }
        [JsonPropertyName("ulica")]
        public string Ulica { get; set; }
        [JsonPropertyName("numer_domu")]
        public string NrDomu { get; set; }
        [JsonPropertyName("numer_mieszkania")]
        public int? NrMieszkania  { get; set; }
        [JsonPropertyName("kod_pocztowy")]
        public string KodPocztowy { get; set; }
        [JsonPropertyName("miasto")]
        public string Miasto { get; set; }
        [JsonPropertyName("typ_adresu")]
        public string TypAdresu { get; set; }
        [JsonPropertyName("id_klienta")]
        public int KlientId { get; set; }
        public Klient Klient { get; set; }
        public string PelnyAdres => $"{Ulica} {NrDomu} {NrMieszkania} {KodPocztowy} {Miasto} {TypAdresu}";
    }
}
