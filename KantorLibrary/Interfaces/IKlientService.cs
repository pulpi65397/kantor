using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Interfaces
{
    public interface IKlientService
    {
        IEnumerable<Klient> GetAll();
        Klient GetById(int id);
        void Add(Klient klient);
        void Update(Klient klient);
        void Delete(int id);
        bool IsAdmin(int klientId);
    }
}
