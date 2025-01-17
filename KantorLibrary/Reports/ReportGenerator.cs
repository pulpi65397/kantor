using System;
using System.Collections.Generic;
using System.Linq;
using KantorLibrary.Models;
using KantorLibrary.Repositories;
using System.Text;

namespace KantorLibrary.Reports
{
    public interface IReportGenerator
    {
        string GenerateCurrencyTurnoverReport(DateTime startDate, DateTime endDate);
        string GenerateCurrencyRateAnalysisReport(DateTime startDate, DateTime endDate);
        string GenerateCostReport(DateTime startDate, DateTime endDate);
        string GenerateCurrencyPopularityReport(DateTime startDate, DateTime endDate);
        string GenerateProfitLossReport(DateTime startDate, DateTime endDate);
    }

    public class ReportGenerator : IReportGenerator
    {
        private readonly IRepozytorium<Zamowienie> _zamowienieRepo;
        private readonly IRepozytorium<Kurs> _kursRepo;

        public ReportGenerator(IRepozytorium<Zamowienie> zamowienieRepo, IRepozytorium<Kurs> kursRepo)
        {
            _zamowienieRepo = zamowienieRepo;
            _kursRepo = kursRepo;
        }

        // Metoda generująca raport zestawienia obrotów walutowych
        public string GenerateCurrencyTurnoverReport(DateTime startDate, DateTime endDate)
        {
            var transactions = _zamowienieRepo.ToList()  // Zamiana na Listę
                .Where(z => z.Data >= startDate && z.Data <= endDate)
                .GroupBy(z => z.Kurs.Waluta)
                .Select(g => new
                {
                    Currency = g.Key,
                    TotalTurnover = g.Sum(z => z.Wartosc)
                })
                .OrderByDescending(r => r.TotalTurnover)
                .ToList();

            return FormatReport("Zestawienie obrotów walutowych", transactions);
        }

        // Metoda generująca raport analizy kursów walutowych
        public string GenerateCurrencyRateAnalysisReport(DateTime startDate, DateTime endDate)
        {
            var rateChanges = _kursRepo.ToList()  // Zamiana na Listę
                .Select(k => new
                {
                    Currency = k.Waluta,
                    AverageRate = _zamowienieRepo.ToList()
                        .Where(z => z.KursId == k.Id && z.Data >= startDate && z.Data <= endDate)
                        .Average(z => z.CenaKursu)
                })
                .ToList();

            return FormatReport("Analiza kursów walutowych", rateChanges);
        }

        // Metoda generująca raport zestawienia kosztów
        public string GenerateCostReport(DateTime startDate, DateTime endDate)
        {
            var costs = _zamowienieRepo.ToList()  // Zamiana na Listę
                .Where(z => z.Data >= startDate && z.Data <= endDate && z.Strona == 'S')
                .GroupBy(z => z.Kurs.Waluta)
                .Select(g => new
                {
                    Currency = g.Key,
                    TotalCost = g.Sum(z => z.Wartosc)
                })
                .OrderByDescending(r => r.TotalCost)
                .ToList();

            return FormatReport("Zestawienie kosztów", costs);
        }

        // Metoda generująca raport analizy popularności walut
        public string GenerateCurrencyPopularityReport(DateTime startDate, DateTime endDate)
        {
            var popularity = _zamowienieRepo.ToList()  // Zamiana na Listę
                .Where(z => z.Data >= startDate && z.Data <= endDate)
                .GroupBy(z => z.Kurs.Waluta)
                .Select(g => new
                {
                    Currency = g.Key,
                    TotalTransactions = g.Count()
                })
                .OrderByDescending(r => r.TotalTransactions)
                .ToList();

            return FormatReport("Analiza popularności walut", popularity);
        }

        // Metoda generująca raport zestawienia zysków i strat
        public string GenerateProfitLossReport(DateTime startDate, DateTime endDate)
        {
            var profitLoss = _zamowienieRepo.ToList()  // Zamiana na Listę
                .Where(z => z.Data >= startDate && z.Data <= endDate)
                .GroupBy(z => z.Kurs.Waluta)
                .Select(g => new
                {
                    Currency = g.Key,
                    TotalProfitLoss = g.Sum(z => z.Strona == 'K' ? z.Wartosc : -z.Wartosc)
                })
                .OrderByDescending(r => r.TotalProfitLoss)
                .ToList();

            return FormatReport("Zestawienie zysków i strat", profitLoss);
        }

        // Pomocnicza metoda formatująca raport w formie tekstowej
        private string FormatReport(string title, IEnumerable<object> data)
        {
            var report = new StringBuilder();
            report.AppendLine($"{title}");
            report.AppendLine(new string('-', 50));

            foreach (var item in data)
            {
                foreach (var prop in item.GetType().GetProperties())
                {
                    report.AppendLine($"{prop.Name}: {prop.GetValue(item)}");
                }
                report.AppendLine(new string('-', 50));
            }

            return report.ToString();
        }
    }
}
