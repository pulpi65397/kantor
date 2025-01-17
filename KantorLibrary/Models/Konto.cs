using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KantorLibrary.Models
{
    public class Konto
    {
        public int Id { get; set; }
        public int KlientId { get; set; }
        public Klient Klient { get; set; }
        public string Waluta { get; set; }
        public string Grafika { get; set; }
        public decimal Kwota { get; set; }
        public DateTime Data { get; set; } //data rejestracji

        public void UzupełnijSaldo(decimal kwota)
        {
            if (kwota <= 0)
            {
                throw new ArgumentException("Kwota musi być większa niż zero.");
            }
            Kwota += kwota;
        }

        public void WypłaćSaldo(decimal kwota)
        {
            if (kwota <= 0 || Kwota < kwota)
            {
                throw new ArgumentException("Kwota do wypłaty jest nieprawidłowa.");
            }
            Kwota -= kwota;
        }
    }
}
