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
        private Form1 _form1; 

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

                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    List<Klient> klienci = JsonSerializer.Deserialize<List<Klient>>(jsonContent) ?? new List<Klient>();

                    var klient = klienci.Find(k => k.Login == login && k.Haslo == haslo);
                    if (klient != null)
                    {
                     
                        _form1.Hide();  

                        Form1 form1 = new Form1(klient.Typ);  
                        form1.Show();

                        Form4 form4 = new Form4(klient.Id);
                        form4.Show();  

                        this.Hide();  
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
