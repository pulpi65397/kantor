using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Interfaces
{
    public interface ILokalizacjaService
    {
        IEnumerable<Lokalizacja> GetAll();
        Lokalizacja GetById(int id);
        void Add(Lokalizacja lokalizacja);
        void Update(Lokalizacja lokalizacja);
        void Delete(int id);
    }
}
