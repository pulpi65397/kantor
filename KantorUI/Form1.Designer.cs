using System.Windows.Forms;

namespace KantorUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            listView1 = new ListView();
            registerButton = new Button();
            loginButton = new Button();
            logoutButton = new Button();
            lokalizacjeButton = new Button();
            raportyButton = new Button();
            imageList = new ImageList();

            // Dodanie kolumn do listView1
            listView1.Columns.Add("Grafika", 50);
            listView1.Columns.Add("Waluta", 50);
            listView1.Columns.Add("Kupno", 75);
            listView1.Columns.Add("Sprzedaż", 75);
            listView1.SmallImageList = imageList;

            SuspendLayout();

            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 12);
            label1.Name = "label1";
            label1.Size = new Size(168, 15);
            label1.TabIndex = 0;
            label1.Text = "Aktualne kursy wymiany walut";

            // 
            // listView1
            // 
            listView1.Location = new Point(24, 40);
            listView1.Name = "listView1";
            listView1.Size = new Size(275, 275);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;

            // 
            // registerButton
            // 
            registerButton.Location = new Point(300, 38);
            registerButton.Name = "registerButton";
            registerButton.Size = new Size(100, 23);
            registerButton.TabIndex = 2;
            registerButton.Text = "Zarejestruj się";
            registerButton.UseVisualStyleBackColor = true;
            registerButton.Click += new EventHandler(this.registerButton_Click);  // Przypisanie zdarzenia kliknięcia

            // 
            // loginButton
            // 
            loginButton.Location = new Point(300, 67);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(100, 23);
            loginButton.TabIndex = 3;
            loginButton.Text = "Zaloguj się";
            loginButton.UseVisualStyleBackColor = true;
            loginButton.Click += new EventHandler(this.loginButton_Click);  // Przypisanie zdarzenia kliknięcia

            // 
            // logoutButton
            // 
            logoutButton.Location = new Point(300, 67);
            logoutButton.Name = "logoutButton";
            logoutButton.Size = new Size(100, 23);
            logoutButton.TabIndex = 3;
            logoutButton.Text = "Wyloguj się";
            logoutButton.UseVisualStyleBackColor = true;
            logoutButton.Click += new EventHandler(this.logoutButton_Click);  // Przypisanie zdarzenia kliknięcia

            // 
            // lokalizacjeButton
            // 
            lokalizacjeButton.Location = new Point(400, 96);
            lokalizacjeButton.Name = "lokalizacjeButton";
            lokalizacjeButton.Size = new Size(100, 23);
            lokalizacjeButton.TabIndex = 4;
            lokalizacjeButton.Text = "Lokalizacje";
            lokalizacjeButton.UseVisualStyleBackColor = true;
            lokalizacjeButton.Click += new EventHandler(this.lokalizacjeButton_Click);  // Przypisanie zdarzenia kliknięcia
            
            // 
            // raportyButton
            // 
            raportyButton.Location = new Point(400, 125);
            raportyButton.Name = "raportyButton";
            raportyButton.Size = new Size(100, 23);
            raportyButton.TabIndex = 4;
            raportyButton.Text = "Raporty";
            raportyButton.UseVisualStyleBackColor = true;
            raportyButton.Click += new EventHandler(this.raportyButton_Click);  // Przypisanie zdarzenia kliknięcia

            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(405, 325);
            Controls.Add(raportyButton);
            Controls.Add(lokalizacjeButton);
            Controls.Add(loginButton);
            Controls.Add(registerButton);
            Controls.Add(logoutButton);
            Controls.Add(listView1);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Label label1;
        private ListView listView1;
        private Button registerButton;
        private Button loginButton;
        private Button logoutButton;
        private Button lokalizacjeButton;
        private Button raportyButton;
        private ImageList imageList;
    }
}