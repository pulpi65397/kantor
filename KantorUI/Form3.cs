using KantorLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace KantorUI
{
    public partial class Form3 : Form
    {
        private Form1 _form1;  // Referencja do Form1

        // Przekazanie instancji Form1 do Form3 w konstruktorze
        public Form3(Form1 form1)
        {
            InitializeComponent();
            _form1 = form1;
            textBox2.PasswordChar = '*';
        }

        private void Login(string login, string haslo)
        {
            try
            {
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "klienci.json");

                // Wczytanie danych z pliku JSON
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    List<Klient> klienci = JsonSerializer.Deserialize<List<Klient>>(jsonContent) ?? new List<Klient>();

                    // Wyszukiwanie klienta
                    var klient = klienci.Find(k => k.Login == login && k.Haslo == haslo);
                    if (klient != null)
                    {
                        // Logowanie udane, otwarcie Form1 i Form4
                        _form1.Hide();  // Ukrycie Form1 (użytkownik niezalogowany)

                        Form1 form1 = new Form1(klient.Typ);  // Tworzenie nowego Form1 z odpowiednim typem użytkownika
                        form1.Show();  // Pokazywanie nowej wersji Form1

                        Form4 form4 = new Form4(klient.Id);  // Tworzenie Form4 z Id klienta
                        form4.Show();  // Pokazywanie Form4

                        this.Hide();  // Ukrycie Form3
                    }
                    else
                    {
                        MessageBox.Show("Niepoprawne dane logowania. Spróbuj ponownie.");
                    }
                }
                else
                {
                    MessageBox.Show("Baza danych użytkowników nie istnieje.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas logowania: {ex.Message}");
            }
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string haslo = textBox2.Text;
            Login(login, haslo);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
        }
    }
}
