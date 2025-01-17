using System.Collections.Generic;
using System.Threading.Tasks;

namespace KantorLibrary.Repositories
{
    public interface IRepozytorium<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T item);
        void Update(T item);
        void Delete(int id);

        List<T> ToList();
    }
}
