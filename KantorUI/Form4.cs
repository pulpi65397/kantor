using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using KantorLibrary.Models;
using KantorLibrary.Services;


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

                this.zamowienia = zamowienia;
                this.kursy = kursy;
                this.lokalizacje = lokalizacje;
                this.adresy = adresy;

                if (konta == null || kursy == null || lokalizacje == null || adresy == null || zamowienia == null)
                {
                    MessageBox.Show("Błąd podczas ładowania danych.");
                    return;
                }

                // Dodanie pustych opcji do combobox4, combobox5, combobox6
                kursy.Insert(0, new Kurs { Id = 0, Waluta = "" });

                var stronyOpisowe = new List<string> { "", "Kupno", "Sprzedaż" };

                var sortOptions = new List<string>
        {
            "",
            "Data zamówienia (rosnąco)",
            "Data zamówienia (malejąco)",
            "Wartość (rosnąco)",
            "Wartość (malejąco)",
            "Ilość (rosnąco)",
            "Ilość (malejąco)",
            "Kurs (rosnąco)",
            "Kurs (malejąco)",
            "Strona (rosnąco)",
            "Strona (malejąco)"
        };

                comboBox1.DataSource = kursy;
                comboBox1.DisplayMember = "Waluta";
                comboBox1.ValueMember = "Id";

                comboBox2.DataSource = lokalizacje;
                comboBox2.DisplayMember = "Kantor";
                comboBox2.ValueMember = "Id";

                comboBox3.DataSource = adresy;
                comboBox3.DisplayMember = "PelnyAdres";
                comboBox3.ValueMember = "Id";

                comboBox4.DataSource = kursy;
                comboBox4.DisplayMember = "Waluta";
                comboBox4.ValueMember = "Id";

                comboBox5.DataSource = stronyOpisowe;

                comboBox6.DataSource = sortOptions;

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
                    AddAddButton(item, konto);
                    AddSubButton(item, konto);
                }

                label2.Text = $"Suma wartości walut w PLN: {sumaWPln.ToString("C2", CultureInfo.CurrentCulture)}";

                // Wyświetlenie złożonych zamówień w ListView2
                listView2.Items.Clear();
                listView2.Columns.Clear();
                listView2.Columns.Add("Data zamówienia", 125);
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
                        ListViewItem item = new ListViewItem(zamowienie.Data.ToString("yyyy-MM-dd"));
                        item.SubItems.Add(kurs.Waluta);
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
        private void AddAddButton(ListViewItem item, Konto konto)
        {
            Button addFundsButton = new Button
            {
                Text = "+",
                Tag = konto,
                Size = new Size(20, 20),
                Location = new Point(item.Bounds.Right, item.Bounds.Top)
            };
            addFundsButton.Click += AddFundsButton_Click;
            listView1.Controls.Add(addFundsButton);
        }
        private void AddSubButton(ListViewItem item, Konto konto)
        {
            Button subFundsButton = new Button
            {
                Text = "-",
                Tag = konto,
                Size = new Size(20, 20),
                Location = new Point(item.Bounds.Right+20, item.Bounds.Top)
            };
            subFundsButton.Click += SubFundsButton_Click;
            listView1.Controls.Add(subFundsButton);
        }

        private void AddFundsButton_Click(object sender, EventArgs e)
        {
            if (sender is Button addFundsButton && addFundsButton.Tag is Konto konto)
            {
                // Otwieranie okna dodawania środków dla konta walutowego
                ShowAddFundsForm(konto);
            }
        }
        private void SubFundsButton_Click(object sender, EventArgs e)
        {
            if (sender is Button subFundsButton && subFundsButton.Tag is Konto konto)
            {
                ShowSubFundsForm(konto);
            }
        }
        private void ShowAddFundsForm(Konto konto)
        {
            // Tworzenie formularza dodawania środków
            Form addFundsForm = new Form
            {
                Text = $"Dodaj środki na konto: {konto.Waluta}",
                Size = new Size(300, 200)
            };

            Label amountLabel = new Label
            {
                Text = "Kwota:",
                Location = new Point(10, 10)
            };

            TextBox amountTextBox = new TextBox
            {
                Location = new Point(10, 40),
                Width = 100
            };

            Button saveButton = new Button
            {
                Text = "Dodaj",
                Location = new Point(10, 80)
            };

            saveButton.Click += (sender, args) =>
            {
                try
                {
                    // Pobieranie kwoty do dodania
                    if (!decimal.TryParse(amountTextBox.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal amountToAdd))
                    {
                        MessageBox.Show("Wprowadź poprawną kwotę.");
                        return;
                    }

                    if (amountToAdd <= 0)
                    {
                        MessageBox.Show("Kwota musi być większa od zera.");
                        return;
                    }

                    // Aktualizacja środków na koncie
                    konto.Kwota += amountToAdd;

                    // Aktualizacja w pliku JSON
                    UpdateAccountInJson(konto);

                    MessageBox.Show($"Środki zostały dodane na konto: {konto.Waluta}");

                    // Odświeżenie widoku
                    RefreshListView();

                    addFundsForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}");
                }
            };

            addFundsForm.Controls.Add(amountLabel);
            addFundsForm.Controls.Add(amountTextBox);
            addFundsForm.Controls.Add(saveButton);
            addFundsForm.ShowDialog();
        }
        private void ShowSubFundsForm(Konto konto)
        {
            Form subFundsForm = new Form
            {
                Text = $"Wypłać środki z konta: {konto.Waluta}",
                Size = new Size(300, 200)
            };

            Label amountLabel = new Label
            {
                Text = "Kwota:",
                Location = new Point(10, 10)
            };

            TextBox amountTextBox = new TextBox
            {
                Location = new Point(10, 40),
                Width = 100
            };

            Button saveButton = new Button
            {
                Text = "Wypłać",
                Location = new Point(10, 80)
            };

            saveButton.Click += (sender, args) =>
            {
                try
                {
                    if (!decimal.TryParse(amountTextBox.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal amountToSubstract))
                    {
                        MessageBox.Show("Wprowadź poprawną kwotę.");
                        return;
                    }

                    if (amountToSubstract <= 0)
                    {
                        MessageBox.Show("Kwota musi być większa od zera.");
                        return;
                    }

                    // Aktualizacja środków na koncie
                    konto.Kwota -= amountToSubstract;

                    // Aktualizacja w pliku JSON
                    UpdateAccountInJson(konto);

                    MessageBox.Show($"Środki zostały wypłacone z konta: {konto.Waluta}");

                    // Odświeżenie widoku
                    RefreshListView();

                    subFundsForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}");
                }
            };

            subFundsForm.Controls.Add(amountLabel);
            subFundsForm.Controls.Add(amountTextBox);
            subFundsForm.Controls.Add(saveButton);
            subFundsForm.ShowDialog();
        }

        private void UpdateAccountInJson(Konto konto)
        {
            try
            {
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string jsonFilePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");

                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show("Plik z danymi kont nie istnieje.");
                    return;
                }

                string jsonContent = File.ReadAllText(jsonFilePath);
                var konta = JsonSerializer.Deserialize<List<Konto>>(jsonContent) ?? new List<Konto>();

                var kontoDoAktualizacji = konta.FirstOrDefault(k => k.Id == konto.Id);
                if (kontoDoAktualizacji != null)
                {
                    kontoDoAktualizacji.Kwota = konto.Kwota;

                    jsonContent = JsonSerializer.Serialize(konta, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(jsonFilePath, jsonContent);
                }
                else
                {
                    MessageBox.Show("Nie znaleziono konta do aktualizacji.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas aktualizacji pliku JSON: {ex.Message}");
            }
        }

        private void RefreshListView()
        {
            try
            {
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string jsonFilePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");

                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show("Plik z danymi kont nie istnieje.");
                    return;
                }

                string jsonContent = File.ReadAllText(jsonFilePath);
                var konta = JsonSerializer.Deserialize<List<Konto>>(jsonContent) ?? new List<Konto>();

                // Filtrowanie kont dla danego klienta
                var kontaDlaKlienta = konta.Where(k => k.KlientId == KlientId).ToList();

                listView1.Items.Clear();

                if (kontaDlaKlienta.Count == 0)
                {
                    MessageBox.Show("Brak kont dla wskazanego klienta.");
                    return;
                }

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
                    AddAddButton(item, konto);
                    AddSubButton(item, konto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas odświeżania listy: {ex.Message}");
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

                var walutaObj = (Kurs)comboBox4.SelectedItem;
                string waluta = walutaObj.Waluta;
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

                
                var filteredZamowienia = zamowienia
                    .Where(z => z.KursId == walutaObj.Id)
                    .Where(z => z.Strona == strona[0])
                    .Where(z => z.Data.Date >= dataOd.Date)
                    .Where(z => z.Data.Date <= dataDo.Date)
                    .ToList();
                // Zadeklarowanie zmiennej przed użyciem
                IOrderedEnumerable<Zamowienie> sortedZamowienia;

                // Sortowanie danych
                string sortOption = comboBox6.SelectedItem.ToString();

                switch (sortOption)
                {
                    case "Data zamówienia (rosnąco)":
                        sortedZamowienia = zamowienia.OrderBy(z => z.Data);
                        break;
                    case "Data zamówienia (malejąco)":
                        sortedZamowienia = zamowienia.OrderByDescending(z => z.Data);
                        break;
                    case "Wartość (rosnąco)":
                        sortedZamowienia = zamowienia.OrderBy(z => z.Wartosc);
                        break;
                    case "Wartość (malejąco)":
                        sortedZamowienia = zamowienia.OrderByDescending(z => z.Wartosc);
                        break;
                    case "Ilość (rosnąco)":
                        sortedZamowienia = zamowienia.OrderBy(z => z.Ilosc);
                        break;
                    case "Ilość (malejąco)":
                        sortedZamowienia = zamowienia.OrderByDescending(z => z.Ilosc);
                        break;
                    case "Kurs (rosnąco)":
                        sortedZamowienia = zamowienia.OrderBy(z => z.CenaKursu);
                        break;
                    case "Kurs (malejąco)":
                        sortedZamowienia = zamowienia.OrderByDescending(z => z.CenaKursu);
                        break;
                    case "Strona (rosnąco)":
                        sortedZamowienia = zamowienia.OrderBy(z => z.Strona);
                        break;
                    case "Strona (malejąco)":
                        sortedZamowienia = zamowienia.OrderByDescending(z => z.Strona);
                        break;
                    default:
                        sortedZamowienia = zamowienia.OrderBy(z => z.Data); // Domyślnie sortuj po dacie rosnąco
                        break;
                }

                // Zamiana z IOrderedEnumerable na List<Zamowienie>
                var sortedZamowieniaList = sortedZamowienia.ToList();

                // Wyświetlanie posortowanych i przefiltrowanych danych w ListView
                listView2.Items.Clear();
                foreach (var zamowienie in sortedZamowieniaList)
                {
                    var kurs = this.kursy.FirstOrDefault(k => k.Id == zamowienie.KursId);
                    var lokalizacja = lokalizacje.FirstOrDefault(l => l.Id == zamowienie.LokalizacjaId);
                    var adres = adresy.FirstOrDefault(a => a.Id == zamowienie.AdresId);

                    if (kurs != null && lokalizacja != null && adres != null)
                    {
                        ListViewItem item = new ListViewItem(zamowienie.Data.ToString("yyyy-MM-dd"));
                        item.SubItems.Add(kurs.Waluta);
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
