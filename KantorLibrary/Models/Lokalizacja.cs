using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KantorLibrary.Models
{
    public class Lokalizacja
    {
        public int Id { get; set; }
        public string Miasto { get; set; }
        public string KodKraju { get; set; } //np. POL, USA
        public decimal IloscUSD { get; set; }
        public decimal IloscEUR { get; set; }
        public decimal IloscGBP { get; set; }
        public decimal IloscCHF { get; set; }
        public decimal IloscBTC { get; set; }
        public string Kantor => $"{Miasto} {KodKraju}";
    }
}
