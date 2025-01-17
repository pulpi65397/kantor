using System;
using System.Collections.Generic;
using System.Linq;
using KantorLibrary.Models;
using KantorLibrary.Repositories;
using KantorLibrary.Models;
using KantorLibrary.Interfaces;
 
namespace KantorLibrary.Services
{
    public interface IZamowienieService
    {
        IEnumerable<Zamowienie> GetAll();
        Zamowienie GetById(int id);
        void Add(Zamowienie zamowienie);
        void Update(Zamowienie zamowienie);
        void Delete(int id);
    }

    public class ZamowienieService : IZamowienieService
    {
        private readonly JsonRepozytorium<Zamowienie> _zamowienieRepozytorium;

        public ZamowienieService(string filePath)
        {
            _zamowienieRepozytorium = new JsonRepozytorium<Zamowienie>(filePath);
        }

        public IEnumerable<Zamowienie> GetAll()
        {
            return _zamowienieRepozytorium.GetAll();
        }

        public Zamowienie GetById(int id)
        {
            return _zamowienieRepozytorium.GetById(id);
        }

        public void Add(Zamowienie zamowienie)
        {
            // Możesz dodać walidację adresu przed dodaniem
            if (zamowienie == null)
            {
                throw new ArgumentNullException(nameof(zamowienie), "Zamówienie nie może być nullem.");
            }

            _zamowienieRepozytorium.Add(zamowienie);
        }

        public void Update(Zamowienie zamowienie)
        {
            // Możesz dodać walidację przed aktualizacją
            if (zamowienie == null)
            {
                throw new ArgumentNullException(nameof(zamowienie), "Zamówienie nie może być nullem.");
            }

            _zamowienieRepozytorium.Update(zamowienie);
        }

        public void Delete(int id)
        {
            var zamowienie = _zamowienieRepozytorium.GetById(id);
            if (zamowienie != null)
            {
                _zamowienieRepozytorium.Delete(id);
            }
            else
            {
                throw new InvalidOperationException($"Zamówienie o id {id} nie istnieje.");
            }
        }
    }
}
