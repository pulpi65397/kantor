using System;
using System.Collections.Generic;
using System.Linq;
using KantorLibrary.Models;
using KantorLibrary.Repositories;
using KantorLibrary.Models;
using KantorLibrary.Interfaces;

namespace KantorLibrary.Services
{
    public class AdresService : IAdresService
    {
        private readonly JsonRepozytorium<Adres> _adresRepozytorium;

        public AdresService(string filePath)
        {
            _adresRepozytorium = new JsonRepozytorium<Adres>(filePath);
        }

        public IEnumerable<Adres> GetAll()
        {
            return _adresRepozytorium.GetAll();
        }

        public Adres GetById(int id)
        {
            return _adresRepozytorium.GetById(id);
        }

        public void Add(Adres adres)
        {
            // Możesz dodać walidację adresu przed dodaniem
            if (adres == null)
            {
                throw new ArgumentNullException(nameof(adres), "Adres nie może być nullem.");
            }

            _adresRepozytorium.Add(adres);
        }

        public void Update(Adres adres)
        {
            // Możesz dodać walidację przed aktualizacją
            if (adres == null)
            {
                throw new ArgumentNullException(nameof(adres), "Adres nie może być nullem.");
            }

            _adresRepozytorium.Update(adres);
        }

        public void Delete(int id)
        {
            var adres = _adresRepozytorium.GetById(id);
            if (adres != null)
            {
                _adresRepozytorium.Delete(id);
            }
            else
            {
                throw new InvalidOperationException($"Adres o id {id} nie istnieje.");
            }
        }
    }
}
