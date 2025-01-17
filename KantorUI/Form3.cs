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
        public Form3()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }
        private void Login(string login, string haslo)
        {
            try
            {
                // Konstrukcja pełnej ścieżki do pliku JSON
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "klienci.json");

                // Wczytanie danych z pliku JSON
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    List<Klient> klienci = JsonSerializer.Deserialize<List<Klient>>(jsonContent) ?? new List<Klient>();

                    // Sprawdzanie zgodności loginu i hasła
                    var klient = klienci.Find(k => k.Login == login && k.Haslo == haslo);
                    if (klient != null)
                    {
                        // Logowanie udane, otwarcie Form4 i przekazanie idKlienta
                        Form4 form4 = new Form4(klient.Id);  // Przekazanie idKlienta
                        form4.Show();
                        this.Hide();  // Ukrycie bieżącego formularza
                    }
                    else
                    {
                        // Logowanie nieudane
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
