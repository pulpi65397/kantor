using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using KantorLibrary.Models;

namespace KantorUI
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(new string[]
            {
                "Zestawienie obrotów walutowych",
                "Analiza kursów walutowych",
                "Zestawienie kosztów",
                "Analiza popularności walut",
                "Zestawienie zysków i strat"
            });
            comboBox1.SelectedIndex = 0;

            button1.Click += button1_Click;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dataOd = dateTimePicker1.Value.Date;
            DateTime dataDo = dateTimePicker2.Value.Date;
            string typRaportu = comboBox1.SelectedItem?.ToString();

            if (dataOd > dataDo)
            {
                MessageBox.Show("Data początkowa nie może być późniejsza niż końcowa.");
                return;
            }

            string raport = typRaportu switch
            {
                "Zestawienie obrotów walutowych" => GenerujObrotyWalutowe(dataOd, dataDo),
                "Analiza kursów walutowych" => GenerujAnalizeKursow(dataOd, dataDo),
                "Zestawienie kosztów" => GenerujKoszty(dataOd, dataDo),
                "Analiza popularności walut" => GenerujPopularnoscWalut(dataOd, dataDo),
                "Zestawienie zysków i strat" => GenerujZyskiStraty(dataOd, dataDo),
                _ => "Nieznany typ raportu."
            };

            Size oknoSize = typRaportu switch
            {
                "Zestawienie obrotów walutowych" => new Size(150, 150),
                "Analiza kursów walutowych" => new Size(450, 600),
                "Zestawienie kosztów" => new Size(500, 100),
                "Analiza popularności walut" => new Size(250, 150),
                "Zestawienie zysków i strat" => new Size(500, 100),
                _ => new Size(700, 500)
            };

            

            PokazRaportDialog(raport, oknoSize);
        }



        private void PokazRaportDialog(string raport, Size oknoSize)
        {
            Form raportForm = new Form
            {
                Size = oknoSize,
                StartPosition = FormStartPosition.CenterParent
            };

            var richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = raport,
                Font = new Font("Consolas", 10)
            };

            raportForm.Controls.Add(richTextBox);
            raportForm.ShowDialog(this);
        }



        private string GenerujObrotyWalutowe(DateTime od, DateTime do_)
        {
            var zamowienia = WczytajZamowienia();
            var kursy = WczytajKursy();
            var filtr = zamowienia.Where(z => z.Data >= od && z.Data <= do_).ToList();

            if (!filtr.Any())
                return "Zestawienie obrotów walutowych\nBrak danych w wybranym okresie.";

            var grupy = filtr
                .Select(z => new { Waluta = kursy.FirstOrDefault(k => k.Id == z.KursId)?.Waluta, z.Wartosc })
                .Where(x => !string.IsNullOrEmpty(x.Waluta))
                .GroupBy(x => x.Waluta)
                .Select(g => $"{g.Key}: {g.Sum(x => x.Wartosc):N2}")
                .ToList();

            return "Zestawienie obrotów walutowych\n" + string.Join("\n", grupy);
        }

        private string GenerujAnalizeKursow(DateTime od, DateTime do_)
        {
            var zamowienia = WczytajZamowienia();
            var kursy = WczytajKursy();

            var zamowieniaWZakresie = zamowienia
                .Where(z => z.Data >= od && z.Data <= do_)
                .OrderBy(z => z.Data)
                .ToList();

            if (!zamowieniaWZakresie.Any())
                return "Analiza kursów walutowych\nBrak danych kursów walutowych w wybranym okresie.";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Analiza kursów walutowych");
            sb.AppendLine("Data\t\t\tWaluta\tKurs kupna\tKurs sprzedaży");

            foreach (var z in zamowieniaWZakresie)
            {
                var kurs = kursy.FirstOrDefault(k => k.Id == z.KursId);
                if (kurs != null)
                {
                    sb.AppendLine($"{z.Data:yyyy-MM-dd HH:mm:ss}\t{kurs.Waluta}\t{kurs.KursK:F4}\t\t{kurs.KursS:F4}");
                }
            }

            return sb.ToString();
        }

        private string GenerujKoszty(DateTime od, DateTime do_)
        {
            var zamowienia = WczytajZamowienia();
            var filtr = zamowienia.Where(z => z.Data >= od && z.Data <= do_).ToList();

            if (!filtr.Any())
                return "Zestawienie kosztów\nBrak danych w wybranym okresie.";

            decimal sumaKosztow = filtr.Sum(z => z.Wartosc - (z.Ilosc * z.CenaKursu));
            return $"Zestawienie kosztów\nSuma kosztów w okresie od {od:yyyy-MM-dd} do {do_:yyyy-MM-dd}: {sumaKosztow:N2} PLN";
        }

        private string GenerujPopularnoscWalut(DateTime od, DateTime do_)
        {
            var zamowienia = WczytajZamowienia();
            var kursy = WczytajKursy();
            var filtr = zamowienia.Where(z => z.Data >= od && z.Data <= do_).ToList();

            if (!filtr.Any())
                return "Analiza popularności walut\nBrak danych w wybranym okresie.";

            var popularnosc = filtr
                .Select(z => kursy.FirstOrDefault(k => k.Id == z.KursId)?.Waluta)
                .Where(w => !string.IsNullOrEmpty(w))
                .GroupBy(w => w)
                .OrderBy(g => g.Key)
                .ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Analiza popularności walut");
            sb.AppendLine("Waluta\tIlość transakcji");
            foreach (var g in popularnosc)
            {
                sb.AppendLine($"{g.Key}\t{g.Count()}");
            }
            return sb.ToString();
        }

        private string GenerujZyskiStraty(DateTime od, DateTime do_)
        {
            var zamowienia = WczytajZamowienia();
            var filtr = zamowienia.Where(z => z.Data >= od && z.Data <= do_).ToList();

            if (!filtr.Any())
                return "Zestawienie zysków i strat\nBrak danych w wybranym okresie.";

            decimal zysk = filtr.Sum(z => z.Wartosc - (z.Ilosc * z.CenaKursu));
            return $"Zestawienie zysków i strat\nZyski/Straty w okresie od {od:yyyy-MM-dd} do {do_:yyyy-MM-dd}: {zysk:N2}";
        }


        private List<Zamowienie> WczytajZamowienia()
        {
            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
            string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "zamowienia.json");
            if (!File.Exists(filePath)) return new List<Zamowienie>();
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Zamowienie>>(json) ?? new List<Zamowienie>();
        }

        private List<Kurs> WczytajKursy()
        {
            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
            string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "kursy.json");
            if (!File.Exists(filePath)) return new List<Kurs>();
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Kurs>>(json) ?? new List<Kurs>();
        }
    }
}
