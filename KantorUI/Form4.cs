using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using KantorLibrary.Models;

namespace KantorUI
{
    public partial class Form4 : Form
    {
        private int KlientId;

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

                var konta = LoadFromJson<List<Konto>>(filePathKonta);
                var kursy = LoadFromJson<List<Kurs>>(filePathKursy);
                var lokalizacje = LoadFromJson<List<Lokalizacja>>(filePathLokalizacje);
                var adresy = LoadFromJson<List<Adres>>(filePathAdresy);

                // Sprawdzenie, czy dane zostały poprawnie wczytane
                if (konta == null || kursy == null || lokalizacje == null || adresy == null)
                {
                    MessageBox.Show("Błąd podczas ładowania danych.");
                    return;
                }

                // Ładowanie danych do ComboBox
                comboBox1.DataSource = kursy;
                comboBox1.DisplayMember = "Waluta";
                comboBox1.ValueMember = "Id";

                comboBox2.DataSource = lokalizacje;
                comboBox2.DisplayMember = "Kantor";
                comboBox2.ValueMember = "Id";

                comboBox3.DataSource = adresy;
                comboBox3.DisplayMember = "PelnyAdres";
                comboBox3.ValueMember = "Id";

                // Filtrowanie kont dla danego klienta
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

                foreach (var konto in kontaDlaKlienta)
                {
                    ListViewItem item = new ListViewItem
                    {
                        Text = konto.Waluta
                    };

                    item.SubItems.Add(konto.Kwota >= 0
                        ? konto.Kwota.ToString("F2", CultureInfo.InvariantCulture)
                        : "Nieprawidłowe dane. Liczba jest ujemna");

                    listView1.Items.Add(item);
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

                // Pobranie ostatniego ID transakcji
                int lastTransactionId = GetLastTransactionId();
                int newTransactionId = lastTransactionId + 1;
                SaveLastTransactionId(newTransactionId);

                // Wczytanie kursu
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas przetwarzania transakcji: {ex.Message}");
            }
        }
    }
}
