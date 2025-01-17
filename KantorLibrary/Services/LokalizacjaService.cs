using System;
using System.Collections.Generic;
using System.Linq;
using KantorLibrary.Models;
using KantorLibrary.Repositories;
using KantorLibrary.Models;
using KantorLibrary.Interfaces;

namespace KantorLibrary.Services
{
    public interface ILokalizacjaService
    {
        IEnumerable<Lokalizacja> GetAll();
        Lokalizacja GetById(int id);
        void Add(Lokalizacja lokalizacja);
        void Update(Lokalizacja lokalizacja);
        void Delete(int id);
    }

    public class LokalizacjaService : ILokalizacjaService
    {
        private readonly JsonRepozytorium<Lokalizacja> _lokalizacjaRepozytorium;

        public LokalizacjaService(string filePath)
        {
            _lokalizacjaRepozytorium = new JsonRepozytorium<Lokalizacja>(filePath);
        }

        public IEnumerable<Lokalizacja> GetAll()
        {
            return _lokalizacjaRepozytorium.GetAll();
        }

        public Lokalizacja GetById(int id)
        {
            return _lokalizacjaRepozytorium.GetById(id);
        }

        public void Add(Lokalizacja lokalizacja)
        {
            // Możesz dodać walidację adresu przed dodaniem
            if (lokalizacja == null)
            {
                throw new ArgumentNullException(nameof(lokalizacja), "Lokalizacja nie może być nullem.");
            }

            _lokalizacjaRepozytorium.Add(lokalizacja);
        }

        public void Update(Lokalizacja lokalizacja)
        {
            // Możesz dodać walidację przed aktualizacją
            if (lokalizacja == null)
            {
                throw new ArgumentNullException(nameof(lokalizacja), "Lokalizacja nie może być nullem.");
            }

            _lokalizacjaRepozytorium.Update(lokalizacja);
        }

        public void Delete(int id)
        {
            var lokalizacja = _lokalizacjaRepozytorium.GetById(id);
            if (lokalizacja != null)
            {
                _lokalizacjaRepozytorium.Delete(id);
            }
            else
            {
                throw new InvalidOperationException($"Lokalizacja o id {id} nie istnieje.");
            }
        }
    }
}
