using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using KantorLibrary.Models;

namespace KantorUI
{
    public partial class Form1 : Form
    {
        private char userType;

        public Form1(char userType)
        {
            InitializeComponent();
            this.userType = userType;
            LoadData();
            SetupUI();
        }

        private void LoadData()
        {
            try
            {
                string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                string filePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "kursy.json");

                string jsonContent = File.ReadAllText(filePath);
                List<Kurs> kursy = JsonSerializer.Deserialize<List<Kurs>>(jsonContent);

                if (kursy == null || kursy.Count == 0)
                {
                    MessageBox.Show("Brak danych do wyœwietlenia.");
                    return;
                }

                if (listView1.SmallImageList == null)
                {
                    listView1.SmallImageList = new ImageList();
                }

                listView1.SmallImageList.ImageSize = new Size(32, 32);
                listView1.Items.Clear();

                foreach (var kurs in kursy)
                {
                    ListViewItem item = new ListViewItem();
                    if (!string.IsNullOrEmpty(kurs.Grafika))
                    {
                        string imagePath = Path.Combine(projectDirectory, "KantorLibrary", "Images", kurs.Grafika);

                        if (File.Exists(imagePath))
                        {
                            try
                            {
                                Image img = Image.FromFile(imagePath);
                                int imageIndex = listView1.SmallImageList.Images.Count;
                                listView1.SmallImageList.Images.Add(img);
                                item.ImageIndex = imageIndex;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"B³¹d podczas ³adowania obrazu: {ex.Message}");
                            }
                        }
                    }

                    item.SubItems.Add(kurs.Waluta);
                    item.SubItems.Add(kurs.KursK.ToString("0.####", CultureInfo.InvariantCulture));
                    item.SubItems.Add(kurs.KursS.ToString("0.####", CultureInfo.InvariantCulture));
                    listView1.Items.Add(item);

                    if (userType == 'A') // Tylko dla administratora
                    {
                        AddEditButton(item, kurs);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"B³¹d podczas wczytywania danych: {ex.Message}");
            }
        }

        private void SetupUI()
        {
            if (userType == 'U')
            {
                // Poka¿ przyciski logowania i rejestracji
                loginButton.Visible = true;
                registerButton.Visible = true;
                logoutButton.Visible = false;
            }
            else
            {
                // Poka¿ przycisk wylogowania, ukryj logowanie i rejestracjê
                loginButton.Visible = false;
                registerButton.Visible = false;
                logoutButton.Visible = true;
            }
        }

        private void AddEditButton(ListViewItem item, Kurs kurs)
        {
            Button editButton = new Button
            {
                Text = "Edytuj",
                Tag = kurs,
                Size = new Size(60, 20),
                Location = new Point(item.Bounds.Right + 10, item.Bounds.Top)
            };
            editButton.Click += EditButton_Click;
            listView1.Controls.Add(editButton);
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (sender is Button editButton && editButton.Tag is Kurs kurs)
            {
                MessageBox.Show($"Edytuj kurs: {kurs.Waluta}");
                // Otwieranie okna edycji kursów
            }
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            // Implementacja wylogowania i powrotu do trybu niezalogowanego
            MessageBox.Show("Wylogowano");
            this.userType = 'U';
            SetupUI();
            // Opcjonalnie: Odœwie¿ dane bez funkcji administracyjnych
        }
    }
}
