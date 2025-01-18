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
        public string Ulica { get; set; }
        public string NrDomu { get; set; }
        public int? NrMieszkania  { get; set; }
        public string KodPocztowy { get; set; }
        public string Miasto { get; set; }
        public string TypAdresu { get; set; }
        public int KlientId { get; set; }
        public Klient Klient { get; set; }
        public string PelnyAdres => $"{Ulica} {NrDomu} {NrMieszkania} {KodPocztowy} {Miasto} {TypAdresu}";
    }
}
