using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Interfaces
{
    public interface IZamowienieService
    {
        IEnumerable<Zamowienie> GetAll();
        Zamowienie GetById(int id);
        void Add(Zamowienie zamowienie);
        void Update(Zamowienie zamowienie);
        void Delete(int id);
    }
}
