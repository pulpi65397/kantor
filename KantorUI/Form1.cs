using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Form1(char userType)  // Poprawiony konstruktor
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
                    MessageBox.Show("Brak danych do wy�wietlenia.");
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
                                MessageBox.Show($"B��d podczas �adowania obrazu: {ex.Message}");
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
                MessageBox.Show($"B��d podczas wczytywania danych: {ex.Message}");
            }
        }

        private void SetupUI()
        {
            if (userType == 'U')  // U�ytkownik niezalogowany
            {
                // Poka� przyciski logowania i rejestracji
                loginButton.Visible = true;
                registerButton.Visible = true;
                logoutButton.Visible = false;
            }
            else if (userType == 'K')  // U�ytkownik zalogowany bez uprawnie� administratora
            {
                // Poka� tylko przycisk wylogowania
                loginButton.Visible = false;
                registerButton.Visible = false;
                logoutButton.Visible = true;
            }
            else if (userType == 'A')  // Administrator
            {
                // Poka� tylko przycisk wylogowania oraz rozszerzon� funkcjonalno��
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
                // Otwieranie okna edycji kurs�w dla administratora
                ShowEditForm(kurs);
            }
        }

        private void ShowEditForm(Kurs kurs)
        {
            // Przyk�adowe okno edycji kurs�w
            Form editForm = new Form
            {
                Text = $"Edycja kursu: {kurs.Waluta}",
                Size = new Size(300, 200)
            };

            TextBox buyCourseTextBox = new TextBox
            {
                Text = kurs.KursK.ToString("0.####", CultureInfo.InvariantCulture),
                Location = new Point(10, 10),
                Width = 100
            };

            TextBox sellCourseTextBox = new TextBox
            {
                Text = kurs.KursS.ToString("0.####", CultureInfo.InvariantCulture),
                Location = new Point(10, 40),
                Width = 100
            };

            Button saveButton = new Button
            {
                Text = "Zapisz",
                Location = new Point(10, 70)
            };
            saveButton.Click += (sender, args) =>
            {
                // Tutaj mo�esz zapisa� zmienione dane kursu
                kurs.KursK = decimal.Parse(buyCourseTextBox.Text, CultureInfo.InvariantCulture);
                kurs.KursS = decimal.Parse(sellCourseTextBox.Text, CultureInfo.InvariantCulture);
                MessageBox.Show($"Zaktualizowano kursy dla: {kurs.Waluta}");
                editForm.Close();
            };

            editForm.Controls.Add(buyCourseTextBox);
            editForm.Controls.Add(sellCourseTextBox);
            editForm.Controls.Add(saveButton);
            editForm.ShowDialog();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(this);
            form3.Show();
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Form4"] != null)
            {
                Application.OpenForms["Form4"].Close();  // Zamykamy Form4
            }
            // Implementacja wylogowania i powrotu do trybu niezalogowanego
            MessageBox.Show("Wylogowano");
            this.userType = 'U';  // Zmiana typu u�ytkownika na niezalogowanego
            SetupUI();  // Zaktualizowanie interfejsu
            // Opcjonalnie: Od�wie� dane bez funkcji administracyjnych
        }
    }
}
