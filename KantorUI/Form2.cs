using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace KantorUI
{
    public partial class Form2 : Form
    {
        private string avatarPath = string.Empty;

        // Ścieżka do pliku, który przechowuje ostatni użyty ID
        private string idFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastClientId.txt");

        public Form2()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }

        // Metoda do pobrania ostatniego ID z pliku (lub 0, jeśli brak)
        private int GetLastClientId()
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
        private void SaveLastClientId(int id)
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

        private void RegisterClient(string login, string haslo, string imie, string nazwisko,
                            string nazwaFirmy, string nip, string email, string telefon,
                            string avatarPath)
        {
            try
            {
                // Walidacja: Jeśli jedno z pól (nazwa firmy lub nip) jest wypełnione, drugie musi być również
                if (!string.IsNullOrEmpty(nazwaFirmy) && string.IsNullOrEmpty(nip))
                {
                    MessageBox.Show("Jeśli podajesz nazwę firmy, musisz podać również NIP.");
                    return;
                }

                if (string.IsNullOrEmpty(nazwaFirmy) && !string.IsNullOrEmpty(nip))
                {
                    MessageBox.Show("Jeśli podajesz NIP, musisz podać również nazwę firmy.");
                    return;
                }

                // Pobranie ostatniego ID i inkrementacja
                int lastClientId = GetLastClientId();
                int newClientId = lastClientId + 1;

                // Zapisanie nowego ostatniego ID
                SaveLastClientId(newClientId);

                // Data rejestracji
                DateTime dataRejestracji = DateTime.Now;

                // Tworzenie nowego obiektu klienta
                Klient nowyKlient = new Klient
                {
                    Id = newClientId,
                    Login = login,
                    Haslo = haslo,
                    Imie = imie,
                    Nazwisko = nazwisko,
                    Data = dataRejestracji,
                    NazwaFirmy = nazwaFirmy,
                    NIP = nip,
                    Email = email,
                    Telefon = telefon,
                    Avatar = string.IsNullOrEmpty(avatarPath) ? null : avatarPath,  // Avatar może być null, jeśli nie wybrano
                    Typ = 'K'  // Typ konta domyślnie to 'K'
                };

                // Ścieżka do pliku JSON
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "klienci.json");

                // Wczytanie danych z pliku (jeśli istnieje)
                List<Klient> klienci = new List<Klient>();
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    klienci = JsonSerializer.Deserialize<List<Klient>>(jsonContent) ?? new List<Klient>();
                }

                // Dodanie nowego klienta do listy
                klienci.Add(nowyKlient);

                // Zapisanie zaktualizowanej listy klientów do pliku
                string updatedJsonContent = JsonSerializer.Serialize(klienci, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJsonContent);

                // Tworzenie kont walutowych dla nowego klienta
                CreateCurrencyAccounts(newClientId);

                MessageBox.Show("Rejestracja zakończona pomyślnie!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas rejestracji: {ex.Message}");
            }
        }

        // Metoda do tworzenia kont walutowych
        private void CreateCurrencyAccounts(int clientId)
        {
            try
            {
                // Lista walut, w których będą tworzone konta
                List<string> currencies = new List<string> { "PLN", "USD", "EUR", "GBP", "CHF", "BTC" };

                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");

                // Wczytanie danych z pliku (jeśli istnieje)
                List<Konto> konta = new List<Konto>();
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    konta = JsonSerializer.Deserialize<List<Konto>>(jsonContent) ?? new List<Konto>();
                }

                // Tworzenie kont walutowych dla klienta
                foreach (var waluta in currencies)
                {
                    Konto konto = new Konto
                    {
                        KlientId = clientId,
                        Waluta = waluta,
                        Kwota = 0.0m  // Na początek saldo wynosi 0
                    };

                    konta.Add(konto);
                }

                // Zapisanie zaktualizowanej listy kont do pliku
                string updatedJsonContent = JsonSerializer.Serialize(konta, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas tworzenia kont walutowych: {ex.Message}");
            }
        }


        // Obsługa kliknięcia przycisku "fileButton" – wybór pliku awatara
        private void fileButton_Click(object sender, EventArgs e)
        {
            // Tworzenie okna dialogowego do wyboru pliku
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Obrazy (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Ustawienie ścieżki wybranego pliku jako awatar
                avatarPath = openFileDialog.FileName;
                MessageBox.Show("Plik awatara wybrany pomyślnie!");
            }
        }

        // Obsługa kliknięcia przycisku "registerButton" – rejestracja użytkownika
        private void registerButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Pobranie danych z formularza
                string login = textBox1.Text;
                string haslo = textBox2.Text;
                string imie = textBox3.Text;
                string nazwisko = textBox4.Text;
                string nazwaFirmy = textBox5.Text;
                string nip = textBox6.Text;
                string email = textBox7.Text;
                string telefon = textBox8.Text;

                // Sprawdzenie, czy wszystkie pola są wypełnione
                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(haslo) || string.IsNullOrEmpty(imie) || string.IsNullOrEmpty(nazwisko))
                {
                    MessageBox.Show("Wszystkie pola muszą być wypełnione!");
                    return;
                }

                // Wywołanie metody rejestracji
                RegisterClient(login, haslo, imie, nazwisko, nazwaFirmy, nip, email, telefon, avatarPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas rejestracji: {ex.Message}");
            }
        }

        // Obsługa kliknięcia przycisku "resetButton" – czyszczenie formularza
        private void resetButton_Click(object sender, EventArgs e)
        {
            // Czyszczenie pól formularza
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            avatarPath = string.Empty;  // Resetowanie ścieżki awatara

            MessageBox.Show("Formularz został wyczyszczony.");
        }
    }
}
