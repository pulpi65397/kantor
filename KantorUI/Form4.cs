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

                comboBox1.DataSource = kursy;
                comboBox1.DisplayMember = "Waluta";
                comboBox1.ValueMember = "Id";

                comboBox2.DataSource = lokalizacje;
                comboBox2.DisplayMember = "Kantor";
                comboBox2.ValueMember = "Id";

                // Pobranie adresów dla wybranego klienta
                var adresyDlaKlienta = adresy.Where(a => a.KlientId == KlientId).ToList();

                if (adresyDlaKlienta.Count == 0)
                {
                    MessageBox.Show("Brak adresów przypisanych do wskazanego klienta.");
                    comboBox3.DataSource = null; // Ustawienie pustego źródła danych
                }
                else
                {
                    comboBox3.DataSource = adresyDlaKlienta;
                    comboBox3.DisplayMember = "PelnyAdres"; // Kolumna z pełnym adresem
                    comboBox3.ValueMember = "Id"; // Identyfikator adresu
                }

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

                comboBox4.DataSource = null;
                var kursyZPustym = kursy.ToList();
                kursyZPustym.Insert(0, new Kurs { Id = -1, Waluta = "" }); // Pusta wartość
                comboBox4.DataSource = kursyZPustym;
                comboBox4.DisplayMember = "Waluta";
                comboBox4.ValueMember = "Id";

                var strony = zamowienia.Select(z => z.Strona).Distinct().ToList();

                // Zamiana 'K' na "Kupno" i 'S' na "Sprzedaż" + opcja neutralna
                var stronyOpisowe = new List<string> { "" }; // Dodajemy opcję neutralną
                stronyOpisowe.AddRange(strony.Select(strona =>
                    strona == 'K' ? "Kupno" :
                    (strona == 'S' ? "Sprzedaż" : "Nieznane")));

                // Ustawienie źródła danych w ComboBox
                comboBox5.DataSource = stronyOpisowe;

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
                comboBox6.DataSource = sortOptions;

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
                Location = new Point(item.Bounds.Right + 20, item.Bounds.Top)
            };
            subFundsButton.Click += SubFundsButton_Click;
            listView1.Controls.Add(subFundsButton);
        }

        private void AddFundsButton_Click(object sender, EventArgs e)
        {
            if (sender is Button addFundsButton && addFundsButton.Tag is Konto konto)
            {
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
                    konto.Kwota = decimal.Parse(amountTextBox.Text, CultureInfo.InvariantCulture);

                    string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                    string jsonFilePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var konta = JsonSerializer.Deserialize<List<Konto>>(jsonContent);



                    var kontoDoAktualizacji = konta.FirstOrDefault(k => k.Waluta == konto.Waluta && k.KlientId == KlientId);
                    if (kontoDoAktualizacji != null)
                    {
                        kontoDoAktualizacji.Kwota += konto.Kwota;

                        jsonContent = JsonSerializer.Serialize(konta, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(jsonFilePath, jsonContent);

                        MessageBox.Show($"Zaktualizowano dla: {konto.Waluta}");

                        UpdateKontaListView(konta, KlientId);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono konta do aktualizacji.");
                    }

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
                    konto.Kwota = decimal.Parse(amountTextBox.Text, CultureInfo.InvariantCulture);

                    string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                    string jsonFilePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var konta = JsonSerializer.Deserialize<List<Konto>>(jsonContent);



                    var kontoDoAktualizacji = konta.FirstOrDefault(k => k.Waluta == konto.Waluta && k.KlientId == KlientId);
                    if (kontoDoAktualizacji != null)
                    {
                        kontoDoAktualizacji.Kwota -= konto.Kwota;

                        jsonContent = JsonSerializer.Serialize(konta, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(jsonFilePath, jsonContent);

                        MessageBox.Show($"Zaktualizowano konto: {konto.Waluta}");

                        UpdateKontaListView(konta, KlientId);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono konta do aktualizacji.");
                    }

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

        public async void UpdateKontaListView(List<Konto> konta, int klientId)
        {
            try
            {
                // 1. Ponownie wczytaj kursy walut (aby mieć aktualne dane)
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePathKursy = Path.Combine(projectDirectory, "KantorLibrary", "Data", "kursy.json");
                this.kursy = await Task.Run(() => LoadFromJson<List<Kurs>>(filePathKursy));

                // 2. Aktualizuj listę kont na formularzu
                listView1.Items.Clear();
                var kontaDlaKlienta = konta.Where(k => k.KlientId == klientId).ToList();

                foreach (var konto in kontaDlaKlienta)
                {
                    ListViewItem item = new ListViewItem
                    {
                        Text = konto.Waluta
                    };
                    item.SubItems.Add(konto.Kwota.ToString("F2", CultureInfo.InvariantCulture));
                    listView1.Items.Add(item);
                    AddAddButton(item, konto);
                    AddSubButton(item, konto);
                }

                // 3. Przelicz sumę z uwzględnieniem aktualnych kursów
                decimal sumaWPln = kontaDlaKlienta.Sum(k =>
                    k.Kwota * (kursy.FirstOrDefault(kurs => kurs.Waluta == k.Waluta)?.KursS ?? 0
                ));

                // 4. Aktualizuj widok sumy (bez kontekstu synchronizacji)
                label2.Text = $"Suma wartości walut w PLN: {sumaWPln.ToString("C2", CultureInfo.CurrentCulture)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd aktualizacji: {ex.Message}");
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

        private async void button1_Click(object sender, EventArgs e)
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
                List<Konto> konta = await Task.Run(() => LoadFromJson<List<Konto>>(filePathKonta));
                var kontoKlientaPLN = konta?.FirstOrDefault(k => k.KlientId == KlientId && k.Waluta == "PLN");
                var kontoKlientaObce = konta?.FirstOrDefault(k => k.KlientId == KlientId && k.Waluta != "PLN");
                string filePathKursy = Path.Combine(projectDirectory, "KantorLibrary", "Data", "kursy.json");
                List<Kurs> kursy = await Task.Run(() => LoadFromJson<List<Kurs>>(filePathKursy));

                if (strona == 'K' && kontoKlientaPLN == null)
                {
                    MessageBox.Show("Nie znaleziono konta w walucie PLN.");
                    return;
                }

                if (strona == 'S' && kontoKlientaObce == null)
                {
                    MessageBox.Show("Nie znaleziono konta w wybranej walucie obcej.");
                    return;
                }

                // Sprawdzenie, czy ilość nie przekracza dostępnych środków
                if (strona == 'K' && ilosc * kursy.First(k => k.Id == wybranyKursId).KursK > kontoKlientaPLN.Kwota)
                {
                    MessageBox.Show("Nie masz wystarczających środków w PLN do realizacji tej transakcji.");
                    return;
                }

                if (strona == 'S' && ilosc > kontoKlientaObce.Kwota)
                {
                    MessageBox.Show("Nie masz wystarczających środków w walucie obcej do realizacji tej transakcji.");
                    return;
                }

                // Pobranie ostatniego ID transakcji
                int lastTransactionId = await Task.Run(() => GetLastTransactionId());
                int newTransactionId = lastTransactionId + 1;
                await Task.Run(() => SaveLastTransactionId(newTransactionId));

                // Wczytanie kursu
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
                await Task.Run(() => zamowienieService.Add(zamowienie));

                // Aktualizacja salda
                if (strona == 'K')
                {
                    kontoKlientaPLN.Kwota -= transakcjaWartosc;
                    var kontoWalutaObca = konta.FirstOrDefault(k => k.KlientId == KlientId && k.Waluta == wybranyKurs.Waluta);
                    if (kontoWalutaObca == null)
                    {
                        kontoWalutaObca = new Konto
                        {
                            KlientId = KlientId,
                            Waluta = wybranyKurs.Waluta,
                            Kwota = ilosc
                        };
                        konta.Add(kontoWalutaObca);
                    }
                    else
                    {
                        kontoWalutaObca.Kwota += ilosc;
                    }
                }
                else if (strona == 'S')
                {
                    kontoKlientaObce.Kwota -= ilosc;
                    kontoKlientaPLN.Kwota += transakcjaWartosc;
                }

                await Task.Run(() => SaveToJson(filePathKonta, konta));

                WyswietlZamowienia();
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
                string strona = comboBox5.SelectedItem?.ToString() ?? "";

                // Sprawdzenie, czy daty są poprawnie ustawione
                if (dataOd == null || dataDo == null)
                {
                    MessageBox.Show("Proszę wybrać zakres dat.");
                    return;
                }
               
                var filteredZamowienia = zamowienia
                    .Where(z => walutaObj.Id == -1 || z.KursId == walutaObj.Id)
                    // Jeśli wybrano "Wszystkie", pomiń filtr strony
                    .Where(z => strona == "" || z.Strona == strona[0])
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
                List<Zamowienie> sortedZamowieniaList = sortedZamowienia.ToList();

                WyswietlZamowienia(sortedZamowieniaList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas filtrowania i sortowania danych: {ex.Message}");
            }
        }
        /// <summary>
        /// Wyświetla zamówienia w kontrolce ListView. 
        /// Jeśli przekazano listę zamówień jako parametr, metoda wykorzystuje ją do generowania widoku. 
        /// W przeciwnym razie używa domyślnej listy `this.zamowienia`.
        /// </summary>
        /// <param name="zamowieniaList">
        /// Opcjonalna lista zamówień (`List<Zamowienie>`). Jeśli `null`, metoda użyje `this.zamowienia`.
        /// </param>
        public void WyswietlZamowienia(List<Zamowienie> zamowieniaList = null)
        {
            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
            string filePathZamowienia = Path.Combine(projectDirectory, "KantorLibrary", "Data", "zamowienia.json");
            var zamowienia = LoadFromJson<List<Zamowienie>>(filePathZamowienia);

            this.zamowienia = zamowienia;
            // Użyj przekazanej listy lub domyślnej this.zamowienia
            var listaZamowien = (zamowieniaList ?? this.zamowienia)
                        .Where(z => z.KlientId == this.KlientId)
                        .ToList();

            // Wyczyszczenie ListView
            listView2.Items.Clear();

            // Iteracja po zamówieniach i dodawanie ich do ListView
            foreach (var zamowienie in listaZamowien)
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
        private void button3_Click(object sender, EventArgs e)
        {
            ShowAddAdresForm();
        }

        private void ShowAddAdresForm()
        {
            // Tworzenie formularza dodawania środków
            Form addAdresForm = new Form
            {
                Text = "Formularz dodawania adresu:",
                Size = new Size(300, 300)
            };

            Label streetLabel = new Label
            {
                Text = "Ulica:",
                Location = new Point(10, 10)
            };

            TextBox streetTextBox = new TextBox
            {
                Location = new Point(150, 10),
                Width = 100
            };

            Label homeNumberLabel = new Label
            {
                Text = "Numer domu:",
                Location = new Point(10, 50)
            };

            TextBox homeNumberTextBox = new TextBox
            {
                Location = new Point(150, 50),
                Width = 100
            };

            Label flatNumberLabel = new Label
            {
                Text = "Nr mieszkania:",
                Location = new Point(10, 80)
            };

            TextBox flatNumberTextBox = new TextBox
            {
                Location = new Point(150, 80),
                Width = 100
            };

            Label postCodeLabel = new Label
            {
                Text = "Kod pocztowy:",
                Location = new Point(10, 110)
            };

            TextBox postCodeTextBox = new TextBox
            {
                Location = new Point(150, 110),
                Width = 100
            };

            Label cityLabel = new Label
            {
                Text = "Miasto:",
                Location = new Point(10, 140)
            };

            TextBox cityTextBox = new TextBox
            {
                Location = new Point(150, 140),
                Width = 100
            };

            Label addressTypeLabel = new Label
            {
                Text = "Typ adresu:",
                Location = new Point(10, 170)
            };

            var typAdresu = new List<string>
    {
        "Adres zamieszkania",
        "Adres korespondencyjny",
        "Adres firmy"
    };

            ComboBox addressTypeComboBox = new ComboBox
            {
                Location = new Point(150, 170),
                Width = 120
            };

            addressTypeComboBox.Items.AddRange(typAdresu.ToArray());

            Button addButton = new Button
            {
                Text = "Dodaj",
                Location = new Point(10, 200)
            };

            addButton.Click += (sender, args) =>
            {
                string street = streetTextBox.Text.Trim();
                if (string.IsNullOrEmpty(street))
                {
                    MessageBox.Show("Proszę podać ulicę.");
                    return;
                }

                string homeNumber = homeNumberTextBox.Text.Trim();
                if (string.IsNullOrEmpty(homeNumber))
                {
                    MessageBox.Show("Proszę podać numer domu.");
                    return;
                }

                int flatNumber;
                if (!int.TryParse(flatNumberTextBox.Text, out flatNumber))
                {
                    MessageBox.Show("Nieprawidłowy format numeru mieszkania. Wprowadź liczbę całkowitą.");
                    return;
                }

                string postCode = postCodeTextBox.Text.Trim();
                if (string.IsNullOrEmpty(postCode))
                {
                    MessageBox.Show("Proszę podać kod pocztowy.");
                    return;
                }

                string city = cityTextBox.Text.Trim();
                if (string.IsNullOrEmpty(city))
                {
                    MessageBox.Show("Proszę podać miasto.");
                    return;
                }

                string addressType = addressTypeComboBox.SelectedItem?.ToString();

                Adres newAddress = new Adres
                {
                    KlientId = KlientId,
                    Ulica = street,
                    NrDomu = homeNumber,
                    NrMieszkania = flatNumber,
                    KodPocztowy = postCode,
                    Miasto = city,
                    TypAdresu = addressType
                };

                adresy.Add(newAddress);

                int newAddressId = adresy.Max(a => a.Id) + 1;
                newAddress.Id = newAddressId;

                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePathAdresy = Path.Combine(projectDirectory, "KantorLibrary", "Data", "adresy.json");
                List<Adres> existingAddresses = LoadFromJson<List<Adres>>(filePathAdresy);
                existingAddresses.Add(newAddress);
                SaveToJson(filePathAdresy, existingAddresses);

                comboBox3.DataSource = null;
                comboBox3.DataSource = adresy.Where(a => a.KlientId == KlientId).ToList();
                comboBox3.DisplayMember = "PelnyAdres";
                comboBox3.ValueMember = "Id";

                addAdresForm.Close();

                MessageBox.Show("Adres został dodany pomyślnie.");
            };

            addAdresForm.Controls.Add(streetLabel);
            addAdresForm.Controls.Add(streetTextBox);
            addAdresForm.Controls.Add(homeNumberLabel);
            addAdresForm.Controls.Add(homeNumberTextBox);
            addAdresForm.Controls.Add(flatNumberLabel);
            addAdresForm.Controls.Add(flatNumberTextBox);
            addAdresForm.Controls.Add(postCodeLabel);
            addAdresForm.Controls.Add(postCodeTextBox);
            addAdresForm.Controls.Add(cityLabel);
            addAdresForm.Controls.Add(cityTextBox);
            addAdresForm.Controls.Add(addressTypeLabel);
            addAdresForm.Controls.Add(addressTypeComboBox);
            addAdresForm.Controls.Add(addButton);
            addAdresForm.ShowDialog();
        }



        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1)
            {
                MessageBox.Show("Proszę wybrać adres do usunięcia.");
                return;
            }

            int selectedAddressId = (int)comboBox3.SelectedValue;

            Adres addressToRemove = adresy.FirstOrDefault(a => a.Id == selectedAddressId);
            adresy.Remove(addressToRemove);

            comboBox3.DataSource = null;
            comboBox3.DataSource = adresy.Where(a => a.KlientId == KlientId).ToList();

            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
            string filePathAdresy = Path.Combine(projectDirectory, "KantorLibrary", "Data", "adresy.json");
            SaveToJson(filePathAdresy, adresy);
            MessageBox.Show("Adres został usunięty.");

            comboBox3.DataSource = null;
            comboBox3.DataSource = adresy.Where(a => a.KlientId == KlientId).ToList();
            comboBox3.DisplayMember = "PelnyAdres";
            comboBox3.ValueMember = "Id";

        }
    }
}