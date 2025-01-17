using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Interfaces
{
    public interface IAdresService
    {
        IEnumerable<Adres> GetAll();
        Adres GetById(int id);
        void Add(Adres adres);
        void Update(Adres adres);
        void Delete(int id);
    }
}
