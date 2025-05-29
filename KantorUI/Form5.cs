using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using KantorLibrary.Models;
using KantorLibrary.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace KantorUI
{
    public partial class Form5 : Form
    {
        private List<Lokalizacja> lokalizacje;
        private string lokalizacjeFilePath;

        public Form5()
        {
            InitializeComponent();
            LoadData();
        }

        private string idFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastLocalisationId.txt");


        private int GetLastLocalisationId()
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

        private void SaveLastLocalisationId(int id)
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

        private void LoadData()
        {
            try
            {
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePathLokalizacje = Path.Combine(projectDirectory, "KantorLibrary", "Data", "lokalizacje.json");

                var lokalizacje = LoadFromJson<List<Lokalizacja>>(filePathLokalizacje);

                this.lokalizacje = lokalizacje;

                if (lokalizacje == null)
                {
                    MessageBox.Show("Błąd podczas ładowania danych.");
                    return;
                }

                var sortOptions = new List<string>
        {
            "",
            "Miasto (rosnąco)",
            "Miasto (malejąco)",
            "Kod kraju (rosnąco)",
            "Kod kraju (malejąco)",
            "Ilość USD (rosnąco)",
            "Ilość USD (malejąco)",
            "Ilość EUR (rosnąco)",
            "Ilość EUR (malejąco)",
            "Ilość GBP (rosnąco)",
            "Ilość GBP (malejąco)",
            "Ilość CHF (rosnąco)",
            "Ilość CHF (malejąco)",
            "Ilość BTC (rosnąco)",
            "Ilość BTC (malejąco)"
        };

                comboBox1.DataSource = sortOptions;

                comboBox2.DataSource = lokalizacje;
                comboBox2.DisplayMember = "Kantor";
                comboBox2.ValueMember = "Id";

                comboBox3.DataSource = lokalizacje;
                comboBox3.DisplayMember = "Kantor";
                comboBox3.ValueMember = "Id";



                listView1.Items.Clear();
                listView1.Columns.Clear();
                listView1.Columns.Add("Miasto");
                listView1.Columns.Add("Kod kraju");
                listView1.Columns.Add("Ilość USD");
                listView1.Columns.Add("Ilość EUR");
                listView1.Columns.Add("Ilość GBP");
                listView1.Columns.Add("Ilość CHF");
                listView1.Columns.Add("Ilość BTC");

                listView1.View = View.Details;

                foreach (var lokalizacja in lokalizacje)
                {
                    if (lokalizacja != null)
                    {
                        ListViewItem item = new ListViewItem(lokalizacja.Miasto);
                        item.SubItems.Add(lokalizacja.KodKraju);
                        item.SubItems.Add(lokalizacja.IloscUSD.ToString("F2", CultureInfo.InvariantCulture));
                        item.SubItems.Add(lokalizacja.IloscEUR.ToString("F2", CultureInfo.InvariantCulture));
                        item.SubItems.Add(lokalizacja.IloscGBP.ToString("F2", CultureInfo.InvariantCulture));
                        item.SubItems.Add(lokalizacja.IloscCHF.ToString("F2", CultureInfo.InvariantCulture));
                        item.SubItems.Add(lokalizacja.IloscBTC.ToString("F2", CultureInfo.InvariantCulture));
                        listView1.Items.Add(item);
                    }
                }


                listView1.View = View.Details;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania danych: {ex.Message}");
            }
        }
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
        private void SaveToJson<T>(string filePath, T data)
        {
            try
            {
                string jsonContent = JsonSerializer.Serialize(data);
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd przy zapisywaniu danych do pliku: {ex.Message}");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Pobierz dane z kontrolek
                string miasto = textBox1.Text.Trim();
                string kodKraju = textBox2.Text.Trim();

                if (string.IsNullOrWhiteSpace(miasto) || string.IsNullOrWhiteSpace(kodKraju))
                {
                    MessageBox.Show("Wprowadź miasto i kod kraju.");
                    return;
                }

                if (!decimal.TryParse(textBox3.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iloscUSD) || iloscUSD < 0)
                {
                    MessageBox.Show("Wprowadź poprawną, nieujemną ilość USD.");
                    return;
                }
                if (!decimal.TryParse(textBox4.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iloscEUR) || iloscEUR < 0)
                {
                    MessageBox.Show("Wprowadź poprawną, nieujemną ilość EUR.");
                    return;
                }
                if (!decimal.TryParse(textBox5.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iloscGBP) || iloscGBP < 0)
                {
                    MessageBox.Show("Wprowadź poprawną, nieujemną ilość GBP.");
                    return;
                }
                if (!decimal.TryParse(textBox6.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iloscCHF) || iloscCHF < 0)
                {
                    MessageBox.Show("Wprowadź poprawną, nieujemną ilość CHF.");
                    return;
                }
                if (!decimal.TryParse(textBox7.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal iloscBTC) || iloscBTC < 0)
                {
                    MessageBox.Show("Wprowadź poprawną, nieujemną ilość BTC.");
                    return;
                }

                // Ścieżka do pliku
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePathLokalizacje = Path.Combine(projectDirectory, "KantorLibrary", "Data", "lokalizacje.json");
                string idFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastLocalisationId.txt");

                // Wczytaj istniejące lokalizacje
                List<Lokalizacja> lokalizacje = LoadFromJson<List<Lokalizacja>>(filePathLokalizacje) ?? new List<Lokalizacja>();

                // Wygeneruj nowe ID
                int lastLocalisationId = lokalizacje.Any() ? lokalizacje.Max(l => l.Id) : 0;
                int newLocalisationId = lastLocalisationId + 1;
                File.WriteAllText(idFilePath, newLocalisationId.ToString());

                // Utwórz nową lokalizację
                var lokalizacja = new Lokalizacja
                {
                    Id = newLocalisationId,
                    Miasto = miasto,
                    KodKraju = kodKraju,
                    IloscUSD = iloscUSD,
                    IloscEUR = iloscEUR,
                    IloscGBP = iloscGBP,
                    IloscCHF = iloscCHF,
                    IloscBTC = iloscBTC
                };

                // Dodaj do listy i zapisz do pliku
                lokalizacje.Add(lokalizacja);
                SaveToJson(filePathLokalizacje, lokalizacje);

                MessageBox.Show($"Lokalizacja nr {newLocalisationId} została utworzona.");

                // Odśwież widok
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania lokalizacji: {ex.Message}");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Pobierz wartości filtrów tekstowych
                string miastoFilter = textBox8.Text.Trim();
                string kodKrajuFilter = textBox9.Text.Trim();

                // Pobierz zakresy liczbowych filtrów (od-do)
                decimal.TryParse(textBox11.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal usdOd);
                decimal.TryParse(textBox16.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal usdDo);
                decimal.TryParse(textBox13.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal eurOd);
                decimal.TryParse(textBox18.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal eurDo);
                decimal.TryParse(textBox14.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal gbpOd);
                decimal.TryParse(textBox19.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal gbpDo);
                decimal.TryParse(textBox12.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal chfOd);
                decimal.TryParse(textBox17.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal chfDo);
                decimal.TryParse(textBox10.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal btcOd);
                decimal.TryParse(textBox15.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal btcDo);

                // Wczytaj lokalizacje z pliku
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePathLokalizacje = Path.Combine(projectDirectory, "KantorLibrary", "Data", "lokalizacje.json");
                List<Lokalizacja> lokalizacje = LoadFromJson<List<Lokalizacja>>(filePathLokalizacje) ?? new List<Lokalizacja>();

                // Filtrowanie
                var filtered = lokalizacje.Where(l =>
                    (string.IsNullOrEmpty(miastoFilter) || l.Miasto.Contains(miastoFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(kodKrajuFilter) || l.KodKraju.Contains(kodKrajuFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (usdOd == 0 || l.IloscUSD >= usdOd) &&
                    (usdDo == 0 || l.IloscUSD <= usdDo) &&
                    (eurOd == 0 || l.IloscEUR >= eurOd) &&
                    (eurDo == 0 || l.IloscEUR <= eurDo) &&
                    (gbpOd == 0 || l.IloscGBP >= gbpOd) &&
                    (gbpDo == 0 || l.IloscGBP <= gbpDo) &&
                    (chfOd == 0 || l.IloscCHF >= chfOd) &&
                    (chfDo == 0 || l.IloscCHF <= chfDo) &&
                    (btcOd == 0 || l.IloscBTC >= btcOd) &&
                    (btcDo == 0 || l.IloscBTC <= btcDo)
                );

                // Sortowanie
                string sortOption = comboBox1.SelectedItem?.ToString() ?? "";
                IEnumerable<Lokalizacja> sorted;
                switch (sortOption)
                {
                    case "Miasto (rosnąco)":
                        sorted = filtered.OrderBy(l => l.Miasto);
                        break;
                    case "Miasto (malejąco)":
                        sorted = filtered.OrderByDescending(l => l.Miasto);
                        break;
                    case "Kod kraju (rosnąco)":
                        sorted = filtered.OrderBy(l => l.KodKraju);
                        break;
                    case "Kod kraju (malejąco)":
                        sorted = filtered.OrderByDescending(l => l.KodKraju);
                        break;
                    case "Ilość USD (rosnąco)":
                        sorted = filtered.OrderBy(l => l.IloscUSD);
                        break;
                    case "Ilość USD (malejąco)":
                        sorted = filtered.OrderByDescending(l => l.IloscUSD);
                        break;
                    case "Ilość EUR (rosnąco)":
                        sorted = filtered.OrderBy(l => l.IloscEUR);
                        break;
                    case "Ilość EUR (malejąco)":
                        sorted = filtered.OrderByDescending(l => l.IloscEUR);
                        break;
                    case "Ilość GBP (rosnąco)":
                        sorted = filtered.OrderBy(l => l.IloscGBP);
                        break;
                    case "Ilość GBP (malejąco)":
                        sorted = filtered.OrderByDescending(l => l.IloscGBP);
                        break;
                    case "Ilość CHF (rosnąco)":
                        sorted = filtered.OrderBy(l => l.IloscCHF);
                        break;
                    case "Ilość CHF (malejąco)":
                        sorted = filtered.OrderByDescending(l => l.IloscCHF);
                        break;
                    case "Ilość BTC (rosnąco)":
                        sorted = filtered.OrderBy(l => l.IloscBTC);
                        break;
                    case "Ilość BTC (malejąco)":
                        sorted = filtered.OrderByDescending(l => l.IloscBTC);
                        break;
                    default:
                        sorted = filtered;
                        break;
                }

                // Wyświetl wynik
                WyswietlLokalizacje(sorted.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas filtrowania/sortowania: {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem is Lokalizacja selectedLokalizacja)
            {
                // Utwórz okno edycji "w locie"
                Form editForm = new Form();
                editForm.Text = "Edycja lokalizacji";

                // Pola edycyjne
                var labelMiasto = new Label() { Text = "Miasto:", Left = 10, Top = 20, Width = 80 };
                var textMiasto = new System.Windows.Forms.TextBox() { Left = 100, Top = 20, Width = 150, Text = selectedLokalizacja.Miasto };

                var labelKodKraju = new Label() { Text = "Kod kraju:", Left = 10, Top = 50, Width = 80 };
                var textKodKraju = new System.Windows.Forms.TextBox() { Left = 100, Top = 50, Width = 150, Text = selectedLokalizacja.KodKraju };

                var labelUSD = new Label() { Text = "USD:", Left = 10, Top = 80, Width = 80 };
                var textUSD = new System.Windows.Forms.TextBox() { Left = 100, Top = 80, Width = 150, Text = selectedLokalizacja.IloscUSD.ToString(CultureInfo.InvariantCulture) };

                var labelEUR = new Label() { Text = "EUR:", Left = 10, Top = 110, Width = 80 };
                var textEUR = new System.Windows.Forms.TextBox() { Left = 100, Top = 110, Width = 150, Text = selectedLokalizacja.IloscEUR.ToString(CultureInfo.InvariantCulture) };

                var labelGBP = new Label() { Text = "GBP:", Left = 10, Top = 140, Width = 80 };
                var textGBP = new System.Windows.Forms.TextBox() { Left = 100, Top = 140, Width = 150, Text = selectedLokalizacja.IloscGBP.ToString(CultureInfo.InvariantCulture) };

                var labelCHF = new Label() { Text = "CHF:", Left = 10, Top = 170, Width = 80 };
                var textCHF = new System.Windows.Forms.TextBox() { Left = 100, Top = 170, Width = 150, Text = selectedLokalizacja.IloscCHF.ToString(CultureInfo.InvariantCulture) };

                var labelBTC = new Label() { Text = "BTC:", Left = 10, Top = 200, Width = 80 };
                var textBTC = new System.Windows.Forms.TextBox() { Left = 100, Top = 200, Width = 150, Text = selectedLokalizacja.IloscBTC.ToString(CultureInfo.InvariantCulture) };

                var buttonSave = new System.Windows.Forms.Button() { Text = "Zapisz", Left = 100, Top = 240, Width = 80, DialogResult = DialogResult.OK };
                editForm.AcceptButton = buttonSave;

                editForm.Controls.AddRange(new Control[] {
            labelMiasto, textMiasto,
            labelKodKraju, textKodKraju,
            labelUSD, textUSD,
            labelEUR, textEUR,
            labelGBP, textGBP,
            labelCHF, textCHF,
            labelBTC, textBTC,
            buttonSave
        });

                editForm.ClientSize = new Size(280, 290);

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Zaktualizuj dane lokalizacji
                    selectedLokalizacja.Miasto = textMiasto.Text.Trim();
                    selectedLokalizacja.KodKraju = textKodKraju.Text.Trim();
                    decimal.TryParse(textUSD.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var usd);
                    decimal.TryParse(textEUR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var eur);
                    decimal.TryParse(textGBP.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var gbp);
                    decimal.TryParse(textCHF.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var chf);
                    decimal.TryParse(textBTC.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var btc);
                    selectedLokalizacja.IloscUSD = usd;
                    selectedLokalizacja.IloscEUR = eur;
                    selectedLokalizacja.IloscGBP = gbp;
                    selectedLokalizacja.IloscCHF = chf;
                    selectedLokalizacja.IloscBTC = btc;

                    // Zapisz do pliku
                    string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                    string filePathLokalizacje = Path.Combine(projectDirectory, "KantorLibrary", "Data", "lokalizacje.json");
                    SaveToJson(filePathLokalizacje, lokalizacje);

                    // Odśwież widok
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Wybierz lokalizację do edycji.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem is Lokalizacja selectedLokalizacja)
            {
                var confirm = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć lokalizację: {selectedLokalizacja.Miasto} ({selectedLokalizacja.KodKraju})?",
                    "Potwierdź usunięcie",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    // Usuń z listy
                    lokalizacje.RemoveAll(l => l.Id == selectedLokalizacja.Id);

                    // Zapisz do pliku
                    string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                    string filePathLokalizacje = Path.Combine(projectDirectory, "KantorLibrary", "Data", "lokalizacje.json");
                    SaveToJson(filePathLokalizacje, lokalizacje);

                    // Odśwież widok i comboboxy
                    LoadData();
                    MessageBox.Show("Lokalizacja została usunięta.");
                }
            }
            else
            {
                MessageBox.Show("Wybierz lokalizację do usunięcia.");
            }
        }


        public void WyswietlLokalizacje(List<Lokalizacja> lokalizacjeList = null)
        {
            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
            string filePathLokalizacje = Path.Combine(projectDirectory, "KantorLibrary", "Data", "lokalizacje.json");
            var lokalizacje = LoadFromJson<List<Lokalizacja>>(filePathLokalizacje);

            this.lokalizacje = lokalizacje;
            // Użyj przekazanej listy lub domyślnej this.zamowienia
            var listaLokalizacji = (lokalizacjeList ?? this.lokalizacje).ToList();

            // Wyczyszczenie ListView
            listView1.Items.Clear();

            // Iteracja po zamówieniach i dodawanie ich do ListView
            foreach (var lokalizacja in listaLokalizacji)
            {
                if (lokalizacja != null)
                {
                    ListViewItem item = new ListViewItem(lokalizacja.Miasto);
                    item.SubItems.Add(lokalizacja.KodKraju);
                    item.SubItems.Add(lokalizacja.IloscUSD.ToString("F2", CultureInfo.InvariantCulture));
                    item.SubItems.Add(lokalizacja.IloscEUR.ToString("F2", CultureInfo.InvariantCulture));
                    item.SubItems.Add(lokalizacja.IloscGBP.ToString("F2", CultureInfo.InvariantCulture));
                    item.SubItems.Add(lokalizacja.IloscCHF.ToString("F2", CultureInfo.InvariantCulture));
                    item.SubItems.Add(lokalizacja.IloscBTC.ToString("F2", CultureInfo.InvariantCulture));
                    listView1.Items.Add(item);
                }
            }
        }

    }
}
