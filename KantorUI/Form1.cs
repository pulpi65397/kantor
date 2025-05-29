using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using KantorLibrary.Models;

namespace KantorUI
{
    public partial class Form1 : Form
    {
        private char userType;
        private Size defaultFormSize;
        private Size defaultListViewSize;


        public Form1(char userType)
        {
            InitializeComponent();
            this.userType = userType;
            defaultFormSize = this.Size;
            defaultListViewSize = listView1.Size;
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

                    if (userType == 'A')
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
            if (userType == 'U')
            {

                loginButton.Visible = true;
                registerButton.Visible = true;
                logoutButton.Visible = false;
                lokalizacjeButton.Visible = false;
            }
            else if (userType == 'K')
            {

                loginButton.Visible = false;
                registerButton.Visible = false;
                logoutButton.Visible = true;
                lokalizacjeButton.Visible = false;
            }
            else if (userType == 'A')
            {

                loginButton.Visible = false;
                registerButton.Visible = false;
                logoutButton.Visible = true;
                lokalizacjeButton.Visible = true;
                listView1.Size = new Size(375, 250);
                this.Size = new Size(525, 350);
                logoutButton.Location = new Point(400, 67);
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

                ShowEditForm(kurs);
            }
        }

        private void ShowEditForm(Kurs kurs)
        {

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
                try
                {

                    kurs.KursK = decimal.Parse(buyCourseTextBox.Text, CultureInfo.InvariantCulture);
                    kurs.KursS = decimal.Parse(sellCourseTextBox.Text, CultureInfo.InvariantCulture);

                    string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
                    string jsonFilePath = Path.Combine(projectDirectory, "KantorLibrary", "Data", "kursy.json");
                    string jsonContent = File.ReadAllText(jsonFilePath);
                    var kursy = JsonSerializer.Deserialize<List<Kurs>>(jsonContent);

                    var kursDoAktualizacji = kursy.FirstOrDefault(k => k.Waluta == kurs.Waluta);
                    if (kursDoAktualizacji != null)
                    {
                        kursDoAktualizacji.KursK = kurs.KursK;
                        kursDoAktualizacji.KursS = kurs.KursS;

                        jsonContent = JsonSerializer.Serialize(kursy, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(jsonFilePath, jsonContent);

                        MessageBox.Show($"Zaktualizowano kursy dla: {kurs.Waluta}");

                        UpdateKursyListView(kursy);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono kursu do aktualizacji.");
                    }

                    editForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wyst�pi� b��d: {ex.Message}");
                }
            };

            editForm.Controls.Add(buyCourseTextBox);
            editForm.Controls.Add(sellCourseTextBox);
            editForm.Controls.Add(saveButton);
            editForm.ShowDialog();
        }

        public void UpdateKursyListView(List<Kurs> kursy)
        {
            listView1.Items.Clear();
            foreach (var kurs in kursy)
            {
                var item = new ListViewItem(new[]
                {
            kurs.Waluta,
            kurs.KursK.ToString("0.####", CultureInfo.InvariantCulture),
            kurs.KursS.ToString("0.####", CultureInfo.InvariantCulture)
        });

                if (!string.IsNullOrEmpty(kurs.Grafika))
                {
                    string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
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

                listView1.Items.Add(item);
            }
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

        private void lokalizacjeButton_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["Form4"] != null)
            {
                Application.OpenForms["Form4"].Close();
            }

            MessageBox.Show("Wylogowano");
            this.userType = 'U';
            SetupUI();
            LoadData();
            listView1.Controls.Clear();
            listView1.Size = defaultListViewSize;
            this.Size = defaultFormSize;
        }

    }
}