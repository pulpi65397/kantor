using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Interfaces
{
    public interface IKontoService
    {
        IEnumerable<Konto> GetAll();
        Konto GetById(int id);
        void Add(Konto konto);
        void Update(Konto konto);
        void Delete(int id);
    }
}
