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

        private string idFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lastClientId.txt");

        public Form2()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }

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

                int lastClientId = GetLastClientId();
                int newClientId = lastClientId + 1;

                SaveLastClientId(newClientId);

                DateTime dataRejestracji = DateTime.Now;

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
                    Avatar = string.IsNullOrEmpty(avatarPath) ? null : avatarPath,  
                    Typ = 'K'  
                };


                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "klienci.json");

                List<Klient> klienci = new List<Klient>();
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    klienci = JsonSerializer.Deserialize<List<Klient>>(jsonContent) ?? new List<Klient>();
                }

                klienci.Add(nowyKlient);

                string updatedJsonContent = JsonSerializer.Serialize(klienci, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJsonContent);

                CreateCurrencyAccounts(newClientId);

                MessageBox.Show("Rejestracja zakończona pomyślnie!");
                this.Hide();  
                if (Application.OpenForms["Form1"] != null)
                {
                    Application.OpenForms["Form1"].Hide();
                }
                Form1 form1 = new Form1('K');
                form1.Show();
                Form4 form4 = new Form4(newClientId); 
                form4.Show();  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas rejestracji: {ex.Message}");
            }
        }

        private void CreateCurrencyAccounts(int clientId)
        {
            try
            {

                List<string> currencies = new List<string> { "PLN", "USD", "EUR", "GBP", "CHF", "BTC" };

                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "konta.json");

                List<Konto> konta = new List<Konto>();
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    konta = JsonSerializer.Deserialize<List<Konto>>(jsonContent) ?? new List<Konto>();
                }

                foreach (var waluta in currencies)
                {
                    Konto konto = new Konto
                    {
                        KlientId = clientId,
                        Waluta = waluta,
                        Kwota = 0.0m  
                    };

                    konta.Add(konto);
                }

                string updatedJsonContent = JsonSerializer.Serialize(konta, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJsonContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas tworzenia kont walutowych: {ex.Message}");
            }
        }


        private void fileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Obrazy (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                avatarPath = openFileDialog.FileName;
                MessageBox.Show("Plik awatara wybrany pomyślnie!");
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            try
            {
                string login = textBox1.Text;
                string haslo = textBox2.Text;
                string imie = textBox3.Text;
                string nazwisko = textBox4.Text;
                string nazwaFirmy = textBox5.Text;
                string nip = textBox6.Text;
                string email = textBox7.Text;
                string telefon = textBox8.Text;

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(haslo) || string.IsNullOrEmpty(imie) || string.IsNullOrEmpty(nazwisko))
                {
                    MessageBox.Show("Wszystkie pola muszą być wypełnione!");
                    return;
                }

                RegisterClient(login, haslo, imie, nazwisko, nazwaFirmy, nip, email, telefon, avatarPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas rejestracji: {ex.Message}");
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            avatarPath = string.Empty;  

            MessageBox.Show("Formularz został wyczyszczony.");
        }
    }
}
