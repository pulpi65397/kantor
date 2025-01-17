namespace KantorUI
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label3 = new Label();
            textBox3 = new TextBox();
            label4 = new Label();
            textBox4 = new TextBox();
            label5 = new Label();
            textBox5 = new TextBox();
            label6 = new Label();
            textBox6 = new TextBox();
            label7 = new Label();
            textBox7 = new TextBox();
            label8 = new Label();
            textBox8 = new TextBox();
            label9 = new Label();
            label10 = new Label();
            fileButton = new Button();
            registerButton = new Button();
            resetButton = new Button();
            SuspendLayout();

            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 14);
            label1.Name = "label1";
            label1.Size = new Size(177, 15);
            label1.TabIndex = 0;
            label1.Text = "Pierwszy raz tutaj? Zarejestruj się";

            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(18, 50);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 1;
            label2.Text = "Login:";

            // 
            // textBox1
            // 
            textBox1.Location = new Point(18, 70);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(200, 23);
            textBox1.TabIndex = 2;

            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(18, 100);
            label3.Name = "label3";
            label3.Size = new Size(40, 15);
            label3.TabIndex = 3;
            label3.Text = "Hasło:";

            // 
            // textBox2
            // 
            textBox2.Location = new Point(18, 120);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(200, 23);
            textBox2.TabIndex = 4;

            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(18, 150);
            label4.Name = "label4";
            label4.Size = new Size(33, 15);
            label4.TabIndex = 5;
            label4.Text = "Imię:";

            // 
            // textBox3
            // 
            textBox3.Location = new Point(18, 170);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(200, 23);
            textBox3.TabIndex = 6;

            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(18, 200);
            label5.Name = "label5";
            label5.Size = new Size(60, 15);
            label5.TabIndex = 7;
            label5.Text = "Nazwisko:";

            // 
            // textBox4
            // 
            textBox4.Location = new Point(18, 220);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(200, 23);
            textBox4.TabIndex = 8;

            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(18, 250);
            label6.Name = "label6";
            label6.Size = new Size(76, 15);
            label6.TabIndex = 9;
            label6.Text = "Nazwa firmy:";

            // 
            // textBox5
            // 
            textBox5.Location = new Point(18, 270);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(200, 23);
            textBox5.TabIndex = 10;

            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(18, 300);
            label7.Name = "label7";
            label7.Size = new Size(29, 15);
            label7.TabIndex = 11;
            label7.Text = "NIP:";

            // 
            // textBox6
            // 
            textBox6.Location = new Point(18, 320);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(200, 23);
            textBox6.TabIndex = 12;

            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(18, 350);
            label8.Name = "label8";
            label8.Size = new Size(48, 15);
            label8.TabIndex = 13;
            label8.Text = "Telefon:";

            // 
            // textBox7
            // 
            textBox7.Location = new Point(18, 370);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(200, 23);
            textBox7.TabIndex = 14;

            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(18, 400);
            label9.Name = "label9";
            label9.Size = new Size(44, 15);
            label9.TabIndex = 15;
            label9.Text = "E-mail:";

            // 
            // textBox8
            // 
            textBox8.Location = new Point(18, 420);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(200, 23);
            textBox8.TabIndex = 16;

            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(18, 450);
            label10.Name = "label10";
            label10.Size = new Size(44, 15);
            label10.TabIndex = 17;
            label10.Text = "Avatar:";

            // 
            // fileButton
            // 
            fileButton.Location = new Point(120, 450);
            fileButton.Name = "button1";
            fileButton.Size = new Size(75, 23);
            fileButton.TabIndex = 18;
            fileButton.Text = "Wybierz plik";
            fileButton.UseVisualStyleBackColor = true;
            this.fileButton.Click += new System.EventHandler(this.fileButton_Click);

            // 
            // registerButton
            // 
            registerButton.Location = new Point(18, 480);
            registerButton.Name = "button2";
            registerButton.Size = new Size(96, 23);
            registerButton.TabIndex = 19;
            registerButton.Text = "Zarejestruj się";
            registerButton.UseVisualStyleBackColor = true;
            this.registerButton.Click += new System.EventHandler(this.registerButton_Click);

            // 
            // resetButton
            // 
            resetButton.Location = new Point(120, 480);
            resetButton.Name = "button3";
            resetButton.Size = new Size(75, 23);
            resetButton.TabIndex = 20;
            resetButton.Text = "Resetuj";
            resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);

            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(250, 520);
            Controls.Add(resetButton);
            Controls.Add(registerButton);
            Controls.Add(fileButton);
            Controls.Add(label10);
            Controls.Add(textBox8);
            Controls.Add(label9);
            Controls.Add(textBox7);
            Controls.Add(label8);
            Controls.Add(textBox6);
            Controls.Add(label7);
            Controls.Add(textBox5);
            Controls.Add(label6);
            Controls.Add(textBox4);
            Controls.Add(label5);
            Controls.Add(textBox3);
            Controls.Add(label4);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label3;
        private TextBox textBox3;
        private Label label4;
        private TextBox textBox4;
        private Label label5;
        private TextBox textBox5;
        private Label label6;
        private TextBox textBox6;
        private Label label7;
        private TextBox textBox7;
        private Label label8;
        private TextBox textBox8;
        private Label label9;
        private Label label10;
        private Button fileButton;
        private Button registerButton;
        private Button resetButton;
    }
}