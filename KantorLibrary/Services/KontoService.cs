using System;
using System.Collections.Generic;
using System.Linq;
using KantorLibrary.Models;
using KantorLibrary.Repositories;
using KantorLibrary.Models;
using KantorLibrary.Interfaces;

namespace KantorLibrary.Services
{
    public interface IKontoService
    {
        IEnumerable<Konto> GetAll();
        Konto GetById(int id);
        void Add(Konto konto);
        void Update(Konto konto);
        void Delete(int id);
    }

    public class KontoService : IKontoService
    {
        private readonly JsonRepozytorium<Konto> _kontoRepozytorium;

        public KontoService(string filePath)
        {
            _kontoRepozytorium = new JsonRepozytorium<Konto>(filePath);
        }

        public IEnumerable<Konto> GetAll()
        {
            return _kontoRepozytorium.GetAll();
        }

        public Konto GetById(int id)
        {
            return _kontoRepozytorium.GetById(id);
        }

        public void Add(Konto konto)
        {
            // Możesz dodać walidację adresu przed dodaniem
            if (konto == null)
            {
                throw new ArgumentNullException(nameof(konto), "Konto nie może być nullem.");
            }

            _kontoRepozytorium.Add(konto);
        }

        public void Update(Konto konto)
        {
            // Możesz dodać walidację przed aktualizacją
            if (konto == null)
            {
                throw new ArgumentNullException(nameof(konto), "Konto nie może być nullem.");
            }

            _kontoRepozytorium.Update(konto);
        }

        public void Delete(int id)
        {
            var konto = _kontoRepozytorium.GetById(id);
            if (konto != null)
            {
                _kontoRepozytorium.Delete(id);
            }
            else
            {
                throw new InvalidOperationException($"Konto o id {id} nie istnieje.");
            }
        }
    }
}
