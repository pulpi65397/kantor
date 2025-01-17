using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KantorLibrary.Interfaces
{
    public interface IKursService
    {
        IEnumerable<Kurs> GetAll();
        Kurs GetById(int id);
        void Add(Kurs kurs);
        void Update(Kurs kurs);
        void Delete(int id);
        void UpdateKursValues(int userId, int id, decimal newBuyRate, decimal newSellRate);
    }
}
