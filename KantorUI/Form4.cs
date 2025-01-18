using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using KantorLibrary.Models;
using KantorLibrary.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KantorUI
{
    public partial class Form4 : Form
    {
        private int KlientId;
        private List<Zamowienie> zamowienia;
        private List<Kurs> kursy;
        private List<Lokalizacja> lokalizacje;
        private List<Adres> adresy;

        public Form4(int KlientId)
        {
            InitializeComponent();
            this.KlientId = KlientId;
            LoadData();  // Zamiast wywoływać oddzielnie LoadKonta i LoadAdresy

        }

        private string idFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastTransactionId.txt");

        // Metoda do pobrania ostatniego ID z pliku (lub 0, jeśli brak pliku)
        private int GetLastTransactionId()
        {
            try
            {
                if (File.Exists(idFilePath))
                {
                    string lastIdStr = File.ReadAllText(idFilePath);
                    if (int.TryParse(lastIdStr, out int lastId))
                    {
                        return lastId;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przy odczycie ostatniego ID: {ex.Message}");
            }
            return 0;
        }

        // Metoda do zapisania nowego ostatniego ID do pliku
        private void SaveLastTransactionId(int id)
        {
            try
            {
                File.WriteAllText(idFilePath, id.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przy zapisie ostatniego ID: {ex.Message}");
            }
        }

        // Nowa metoda do ładowania danych
        private void LoadData()
        {
            try
            {
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePathKonta = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");
                string filePathKursy = Path.Combine(projectDirectory, "KantorLibrary", "Data", "kursy.json");
                string filePathLokalizacje = Path.Combine(projectDirectory, "KantorLibrary", "Data", "lokalizacje.json");
                string filePathAdresy = Path.Combine(projectDirectory, "KantorLibrary", "Data", "adresy.json");
                string filePathZamowienia = Path.Combine(projectDirectory, "KantorLibrary", "Data", "zamowienia.json");

                var konta = LoadFromJson<List<Konto>>(filePathKonta);
                var kursy = LoadFromJson<List<Kurs>>(filePathKursy);
                var lokalizacje = LoadFromJson<List<Lokalizacja>>(filePathLokalizacje);
                var adresy = LoadFromJson<List<Adres>>(filePathAdresy);
                var zamowienia = LoadFromJson<List<Zamowienie>>(filePathZamowienia);

                if (konta == null || kursy == null || lokalizacje == null || adresy == null || zamowienia == null)
                {
                    MessageBox.Show("Błąd podczas ładowania danych.");
                    return;
                }

                comboBox1.DataSource = kursy;
                comboBox1.DisplayMember = "Waluta";
                comboBox1.ValueMember = "Id";

                comboBox2.DataSource = lokalizacje;
                comboBox2.DisplayMember = "Kantor";
                comboBox2.ValueMember = "Id";

                comboBox3.DataSource = adresy;
                comboBox3.DisplayMember = "PelnyAdres";
                comboBox3.ValueMember = "Id";

                var kontaDlaKlienta = konta.Where(k => k.KlientId == KlientId).ToList();

                if (kontaDlaKlienta.Count == 0)
                {
                    MessageBox.Show("Brak kont dla wskazanego klienta.");
                    return;
                }

                listView1.Items.Clear();
                listView1.Columns.Clear();
                listView1.Columns.Add("Waluta");
                listView1.Columns.Add("Kwota");

                decimal sumaWPln = 0m;

                foreach (var konto in kontaDlaKlienta)
                {
                    ListViewItem item = new ListViewItem
                    {
                        Text = konto.Waluta
                    };

                    decimal kwota = konto.Kwota;
                    decimal kursDoPln = kursy.FirstOrDefault(k => k.Waluta == konto.Waluta)?.KursS ?? 0;
                    decimal wartoscWPln = kwota * kursDoPln;

                    sumaWPln += wartoscWPln;

                    item.SubItems.Add(kwota >= 0
                        ? kwota.ToString("F2", CultureInfo.InvariantCulture)
                        : "Nieprawidłowe dane. Liczba jest ujemna");

                    listView1.Items.Add(item);
                }

                label2.Text = $"Suma wartości walut w PLN: {sumaWPln.ToString("C2", CultureInfo.CurrentCulture)}";

                comboBox4.DataSource = kursy;
                comboBox4.DisplayMember = "Waluta";
                comboBox4.ValueMember = "Id";

                var strony = zamowienia.Select(z => z.Strona).Distinct().ToList();

                // Zamiana 'K' na "Kupno" i 'S' na "Sprzedaż"
                var stronyOpisowe = strony.Select(strona =>
                    strona == 'K' ? "Kupno" :
                    (strona == 'S' ? "Sprzedaż" : "Nieznane")).ToList();

                // Ustawienie źródła danych w ComboBox
                comboBox5.DataSource = stronyOpisowe;

                var sortOptions = new List<string> {
                    "Data zamówienia (rosnąco)",
                    "Data zamówienia (malejąco)",
                    "Wartość (rosnąco)",
                    "Wartość (malejąco)",
                    "Lokalizacja (rosnąco)",
                    "Lokalizacja (malejąco)",
                    "Waluta (rosnąco)",
                    "Waluta (malejąco)",
                    "Ilość (rosnąco)",
                    "Ilość (malejąco)",
                    "Kurs (rosnąco)",
                    "Kurs (malejąco)",
                    "Strona (rosnąco)",
                    "Strona (malejąco)"

                };
                comboBox6.DataSource = sortOptions;

                // Wyświetlenie złożonych zamówień w ListView2
                listView2.Items.Clear();
                listView2.Columns.Clear();
                listView2.Columns.Add("Waluta");
                listView2.Columns.Add("Ilość");
                listView2.Columns.Add("Wartość");
                listView2.Columns.Add("Lokalizacja", 125);
                listView2.Columns.Add("Adres", 400);
                listView2.Columns.Add("Strona");

                // Ustawienie trybu wyświetlania na szczegóły
                listView2.View = View.Details;

                var zamowieniaDlaKlienta = zamowienia.Where(z => z.KlientId == KlientId).ToList();

                foreach (var zamowienie in zamowieniaDlaKlienta)
                {
                    var kurs = kursy.FirstOrDefault(k => k.Id == zamowienie.KursId);
                    var lokalizacja = lokalizacje.FirstOrDefault(l => l.Id == zamowienie.LokalizacjaId);
                    var adres = adresy.FirstOrDefault(a => a.Id == zamowienie.AdresId);

                    if (kurs != null && lokalizacja != null && adres != null)
                    {
                        ListViewItem item = new ListViewItem(kurs.Waluta);
                        item.SubItems.Add(zamowienie.Ilosc.ToString());
                        item.SubItems.Add(zamowienie.Wartosc.ToString("F2", CultureInfo.InvariantCulture));

                        // Dodanie lokalizacji (miasto i kod kraju)
                        item.SubItems.Add($"{lokalizacja.Miasto} {lokalizacja.KodKraju}");

                        // Dodanie pełnego adresu
                        item.SubItems.Add(adres.PelnyAdres);

                        // Dodanie strony transakcji (Kupno/Sprzedaż)
                        item.SubItems.Add(zamowienie.Strona == 'K' ? "Kupno" : "Sprzedaż");

                        listView2.Items.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania danych: {ex.Message}");
            }
        }




        // Metoda do ogólnego ładowania danych z JSON
        private T LoadFromJson<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    return string.IsNullOrWhiteSpace(jsonContent) ? default : JsonSerializer.Deserialize<T>(jsonContent);
                }
                return default;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przy ładowaniu danych z {filePath}: {ex.Message}");
                return default;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Pobierz dane z ComboBox
                int wybranyKursId = (int)comboBox1.SelectedValue;
                int wybranaLokalizacjaId = (int)comboBox2.SelectedValue;
                int wybranyAdresId = (int)comboBox3.SelectedValue;

                // Pobierz ilość z textBox1 i sprawdź poprawność
                if (!int.TryParse(textBox1.Text, out int ilosc) || ilosc < 0)
                {
                    MessageBox.Show("Wprowadź poprawną, nieujemną ilość.");
                    return;
                }

                // Sprawdzenie zaznaczonego RadioButtona
                char strona = radioButton1.Checked ? 'K' : (radioButton2.Checked ? 'S' : ' ');

                if (strona == ' ')
                {
                    MessageBox.Show("Wybierz stronę transakcji (Kupno/Sprzedaż).");
                    return;
                }

                // Wczytanie danych konta klienta
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePathKonta = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");
                List<Konto> konta = LoadFromJson<List<Konto>>(filePathKonta);
                var kontoKlienta = konta?.FirstOrDefault(k => k.KlientId == KlientId && k.Waluta == comboBox1.Text);

                if (kontoKlienta == null)
                {
                    MessageBox.Show("Nie znaleziono konta dla wybranej waluty.");
                    return;
                }

                // Sprawdzenie, czy ilość nie przekracza dostępnych środków
                if (ilosc > kontoKlienta.Kwota)
                {
                    MessageBox.Show("Nie masz wystarczających środków na koncie do realizacji tej transakcji.");
                    return;
                }

                // Pobranie ostatniego ID transakcji
                int lastTransactionId = GetLastTransactionId();
                int newTransactionId = lastTransactionId + 1;
                SaveLastTransactionId(newTransactionId);

                // Wczytanie kursu
                string filePathKursy = Path.Combine(projectDirectory, "KantorLibrary", "Data", "kursy.json");
                List<Kurs> kursy = LoadFromJson<List<Kurs>>(filePathKursy);

                // Sprawdzanie poprawności kursu
                Kurs wybranyKurs = kursy?.FirstOrDefault(k => k.Id == wybranyKursId);
                if (wybranyKurs == null)
                {
                    MessageBox.Show("Nie znaleziono kursu dla wybranej waluty.");
                    return;
                }

                decimal kursDoUzycia = (strona == 'K') ? wybranyKurs.KursK : wybranyKurs.KursS;
                decimal transakcjaWartosc = ilosc * kursDoUzycia;

                MessageBox.Show($"Transakcja nr {newTransactionId} została zakończona. Kwota: {transakcjaWartosc.ToString("C2", CultureInfo.CurrentCulture)}");

                // Zapisanie transakcji do pliku
                string filePathZamowienia = Path.Combine(projectDirectory, "KantorLibrary", "Data", "zamowienia.json");
                var zamowienie = new Zamowienie
                {
                    Id = newTransactionId,
                    KlientId = KlientId,
                    KursId = wybranyKursId,
                    LokalizacjaId = wybranaLokalizacjaId,
                    AdresId = wybranyAdresId,
                    Ilosc = ilosc,
                    Strona = strona,
                    Wartosc = transakcjaWartosc,
                    Adres = null,
                    CenaKursu = kursDoUzycia,
                    Data = DateTime.Now,
                    Klient = null,
                    Kurs = null,
                    Lokalizacja = null
                };

                ZamowienieService zamowienieService = new ZamowienieService(filePathZamowienia);
                zamowienieService.Add(zamowienie);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas przetwarzania transakcji: {ex.Message}");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Ustawienie domyślnych wartości, jeśli kontrolki są puste
                if (comboBox4.SelectedValue == null) comboBox4.SelectedIndex = 0;  // Ustawienie pierwszej waluty
                if (comboBox5.SelectedValue == null) comboBox5.SelectedIndex = 0;  // Ustawienie pierwszej strony (Kupno/Sprzedaż)
                if (comboBox6.SelectedItem == null) comboBox6.SelectedIndex = 0;  // Ustawienie pierwszej opcji sortowania

                // Pobranie wartości z kontrolek
                DateTime dataOd = dateTimePicker1.Value;
                DateTime dataDo = dateTimePicker2.Value;

                string waluta = comboBox4.SelectedValue.ToString();
                string strona = comboBox5.SelectedValue.ToString();

                // Sprawdzenie, czy daty są poprawnie ustawione
                if (dataOd == null || dataDo == null)
                {
                    MessageBox.Show("Proszę wybrać zakres dat.");
                    return;
                }

                // Jeśli waluta lub strona są puste, ustaw domyślne wartości
                if (string.IsNullOrEmpty(waluta)) waluta = "USD";  // Przykład domyślnej waluty
                if (string.IsNullOrEmpty(strona)) strona = "Kupno";  // Przykład domyślnej strony

                // Filtracja danych
                var filteredZamowienia = zamowienia.Where(z =>
                    z.Data >= dataOd && z.Data <= dataDo &&
                    (string.IsNullOrEmpty(waluta) || z.Kurs.Waluta == waluta) &&
                    (strona == "Kupno" && z.Strona == 'K' || strona == "Sprzedaż" && z.Strona == 'S')
                );

                // Zadeklarowanie zmiennej przed użyciem
                IOrderedEnumerable<Zamowienie> sortedZamowienia;

                // Sortowanie danych
                string sortOption = comboBox6.SelectedItem.ToString();

                switch (sortOption)
                {
                    case "Data zamówienia (rosnąco)":
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.Data);
                        break;
                    case "Data zamówienia (malejąco)":
                        sortedZamowienia = filteredZamowienia.OrderByDescending(z => z.Data);
                        break;
                    case "Wartość (rosnąco)":
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.Wartosc);
                        break;
                    case "Wartość (malejąco)":
                        sortedZamowienia = filteredZamowienia.OrderByDescending(z => z.Wartosc);
                        break;
                    case "Lokalizacja (rosnąco)":
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.Lokalizacja.Miasto);
                        break;
                    case "Lokalizacja (malejąco)":
                        sortedZamowienia = filteredZamowienia.OrderByDescending(z => z.Lokalizacja.Miasto);
                        break;
                    case "Waluta (rosnąco)":
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.Kurs.Waluta);
                        break;
                    case "Waluta (malejąco)":
                        sortedZamowienia = filteredZamowienia.OrderByDescending(z => z.Kurs.Waluta);
                        break;
                    case "Ilość (rosnąco)":
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.Ilosc);
                        break;
                    case "Ilość (malejąco)":
                        sortedZamowienia = filteredZamowienia.OrderByDescending(z => z.Ilosc);
                        break;
                    case "Kurs (rosnąco)":
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.CenaKursu);
                        break;
                    case "Kurs (malejąco)":
                        sortedZamowienia = filteredZamowienia.OrderByDescending(z => z.CenaKursu);
                        break;
                    case "Strona (rosnąco)":
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.Strona);
                        break;
                    case "Strona (malejąco)":
                        sortedZamowienia = filteredZamowienia.OrderByDescending(z => z.Strona);
                        break;
                    default:
                        sortedZamowienia = filteredZamowienia.OrderBy(z => z.Data); // Domyślnie sortuj po dacie rosnąco
                        break;
                }

                // Zamiana z IOrderedEnumerable na List<Zamowienie>
                var sortedZamowieniaList = sortedZamowienia.ToList();

                // Wyświetlanie posortowanych i przefiltrowanych danych w ListView
                listView2.Items.Clear();
                foreach (var zamowienie in sortedZamowieniaList)
                {
                    var kurs = kursy.FirstOrDefault(k => k.Id == zamowienie.KursId);
                    var lokalizacja = lokalizacje.FirstOrDefault(l => l.Id == zamowienie.LokalizacjaId);
                    var adres = adresy.FirstOrDefault(a => a.Id == zamowienie.AdresId);

                    if (kurs != null && lokalizacja != null && adres != null)
                    {
                        ListViewItem item = new ListViewItem(kurs.Waluta);
                        item.SubItems.Add(zamowienie.Ilosc.ToString());
                        item.SubItems.Add(zamowienie.Wartosc.ToString("F2", CultureInfo.InvariantCulture));
                        item.SubItems.Add($"{lokalizacja.Miasto} {lokalizacja.KodKraju}");
                        item.SubItems.Add(adres.PelnyAdres);
                        item.SubItems.Add(zamowienie.Strona == 'K' ? "Kupno" : "Sprzedaż");

                        listView2.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas filtrowania i sortowania danych: {ex.Message}");
            }
        }









    }
}
