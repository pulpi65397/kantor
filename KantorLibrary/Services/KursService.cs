using System;
using System.Collections.Generic;
using KantorLibrary.Models;
using KantorLibrary.Repositories;
using System.Linq;
using KantorLibrary.Interfaces;

namespace KantorLibrary.Services
{
    public class KursService : IKursService
    {
        private readonly JsonRepozytorium<Kurs> _kursRepozytorium;
        private readonly IKlientService _klientService;  // Dodajemy serwis użytkowników

        public KursService(string filePath, IKlientService klientService)
        {
            _kursRepozytorium = new JsonRepozytorium<Kurs>(filePath);
            _klientService = klientService;
        }

        public IEnumerable<Kurs> GetAll()
        {
            return _kursRepozytorium.GetAll();
        }

        public Kurs GetById(int id)
        {
            return _kursRepozytorium.GetById(id);
        }

        public void Add(Kurs kurs)
        {
            if (kurs == null)
            {
                throw new ArgumentNullException(nameof(kurs), "Kurs nie może być nullem.");
            }

            if (_kursRepozytorium.GetAll().Any(k => k.Waluta == kurs.Waluta))
            {
                throw new InvalidOperationException("Kurs tej waluty już istnieje.");
            }

            _kursRepozytorium.Add(kurs);
        }

        public void Update(Kurs kurs)
        {
            if (kurs == null)
            {
                throw new ArgumentNullException(nameof(kurs), "Kurs nie może być nullem.");
            }

            if (_kursRepozytorium.GetById(kurs.Id) == null)
            {
                throw new InvalidOperationException("Kurs nie istnieje w bazie.");
            }

            _kursRepozytorium.Update(kurs);
        }

        public void Delete(int id)
        {
            var kurs = _kursRepozytorium.GetById(id);
            if (kurs != null)
            {
                _kursRepozytorium.Delete(id);
            }
            else
            {
                throw new InvalidOperationException($"Kurs o id {id} nie istnieje.");
            }
        }

        // Nowa metoda do aktualizacji kursu kupna i sprzedaży
        public void UpdateKursValues(int userId, int id, decimal newBuyRate, decimal newSellRate)
        {
            if (!_klientService.IsAdmin(userId))  // Sprawdzamy, czy użytkownik jest administratorem
            {
                throw new UnauthorizedAccessException("Tylko administrator może edytować kursy.");
            }

            if (newBuyRate <= 0 || newSellRate <= 0)
            {
                throw new ArgumentException("Kursy muszą być większe od zera.");
            }

            var kurs = _kursRepozytorium.GetById(id);
            if (kurs != null)
            {
                kurs.KursK = newBuyRate;
                kurs.KursS = newSellRate;
                _kursRepozytorium.Update(kurs);
            }
            else
            {
                throw new InvalidOperationException($"Kurs o id {id} nie istnieje.");
            }
        }
    }
}
