using System;
using System.Collections.Generic;
using KantorLibrary.Models;
using KantorLibrary.Repositories;
using KantorLibrary.Interfaces;

namespace KantorLibrary.Services
{
    // Zaktualizowany interfejs IKlientService
    public interface IKlientService
    {
        IEnumerable<Klient> GetAll();
        Klient GetById(int id);
        void Add(Klient klient);
        void Update(Klient klient);
        void Delete(int id);

        // Dodanie metody IsAdmin do interfejsu
        bool IsAdmin(int klientId);
    }

    public class KlientService : IKlientService
    {
        private readonly JsonRepozytorium<Klient> _klientRepozytorium;

        public KlientService(string filePath)
        {
            _klientRepozytorium = new JsonRepozytorium<Klient>(filePath);
        }

        // Implementacja metody IsAdmin w klasie KlientService
        public bool IsAdmin(int klientId)
        {
            var klient = _klientRepozytorium.GetById(klientId);
            return klient?.Typ == 'A'; // Zakładamy, że 'A' oznacza administratora
        }

        public IEnumerable<Klient> GetAll()
        {
            return _klientRepozytorium.GetAll();
        }

        public Klient GetById(int id)
        {
            return _klientRepozytorium.GetById(id);
        }

        public void Add(Klient klient)
        {
            if (klient == null)
            {
                throw new ArgumentNullException(nameof(klient), "Klient nie może być nullem.");
            }

            _klientRepozytorium.Add(klient);
        }

        public void Update(Klient klient)
        {
            if (klient == null)
            {
                throw new ArgumentNullException(nameof(klient), "Klient nie może być nullem.");
            }

            _klientRepozytorium.Update(klient);
        }

        public void Delete(int id)
        {
            var klient = _klientRepozytorium.GetById(id);
            if (klient != null)
            {
                _klientRepozytorium.Delete(id);
            }
            else
            {
                throw new InvalidOperationException($"Klient o id {id} nie istnieje.");
            }
        }
    }
}
