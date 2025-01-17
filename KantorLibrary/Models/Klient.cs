using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Models
{
    public class Klient
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Haslo { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string? NazwaFirmy { get; set; }
        public string? NIP { get; set; }
        public string? Telefon { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; } // ścieżka do avatara
        public char Typ { get; set; } = 'K';   // admin/user. Przy czym domyślny typ to 'K'
        public DateTime Data { get; set; }  // Data rejestracji


        public ICollection<Adres> Adresy { get; set; }
    }
}
