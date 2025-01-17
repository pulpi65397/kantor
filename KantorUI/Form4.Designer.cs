namespace KantorUI
{
    partial class Form4
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
            listView1 = new ListView();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            comboBox1 = new ComboBox();
            label5 = new Label();
            textBox1 = new TextBox();
            label6 = new Label();
            comboBox2 = new ComboBox();
            comboBox3 = new ComboBox();
            label7 = new Label();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            button1 = new Button();
            label8 = new Label();
            label9 = new Label();
            dateTimePicker1 = new DateTimePicker();
            dateTimePicker2 = new DateTimePicker();
            label10 = new Label();
            comboBox4 = new ComboBox();
            label11 = new Label();
            comboBox5 = new ComboBox();
            label12 = new Label();
            listView2 = new ListView();
            comboBox6 = new ComboBox();
            label13 = new Label();
            listView1.Columns.Add("Waluta", 50);
            listView1.Columns.Add("Kwota", 50);
            SuspendLayout();

            // label1
            label1.AutoSize = true;
            label1.Location = new Point(10, 9);
            label1.Name = "label1";
            label1.Text = "Stan rachunku klienta:";

            // listView1
            listView1.Location = new Point(10, 25);
            listView1.Name = "listView1";
            listView1.Size = new Size(150, 100);
            listView1.Anchor = AnchorStyles.Left;
            listView1.View = View.Details;

            // label2
            label2.AutoSize = true;
            label2.Location = new Point(10, 128);
            label2.Name = "label2";
            label2.Text = "Suma wartości walut obcych w PLN:";

            // label3
            label3.AutoSize = true;
            label3.Location = new Point(10, 143);
            label3.Name = "label3";
            label3.Text = "Formularz zamówienia";

            // label4
            label4.AutoSize = true;
            label4.Location = new Point(10, 158);
            label4.Name = "label4";
            label4.Text = "Waluta:";

            // comboBox1
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(55, 159);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);

            // label5
            label5.AutoSize = true;
            label5.Location = new Point(10, 184);
            label5.Name = "label5";
            label5.Text = "Kwota:";

            // textBox1
            textBox1.Location = new Point(55, 188);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);

            // label6
            label6.AutoSize = true;
            label6.Location = new Point(10, 216);
            label6.Name = "label6";
            label6.Text = "Wybierz lokalizację kantora:";

            // comboBox2
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(160, 216);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(121, 23);

            // comboBox3
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(99, 242);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(375, 23);

            // label7
            label7.AutoSize = true;
            label7.Location = new Point(10, 245);
            label7.Name = "label7";
            label7.Text = "Wybierz adres:";

            // radioButton1
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(10, 270);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(63, 19);
            radioButton1.TabStop = true;
            radioButton1.Text = "Kupno:";
            radioButton1.UseVisualStyleBackColor = true;

            // radioButton2
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(99, 271);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(74, 19);
            radioButton2.TabStop = true;
            radioButton2.Text = "Sprzedaż:";
            radioButton2.UseVisualStyleBackColor = true;

            // button1
            button1.Location = new Point(8, 293);
            button1.Name = "button1";
            button1.Size = new Size(126, 23);
            button1.Text = "Złóż zamówienie";
            button1.UseVisualStyleBackColor = true;
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.button1.Click += new System.EventHandler(this.button1_Click);

            // label8
            label8.AutoSize = true;
            label8.Location = new Point(7, 317);
            label8.Name = "label8";
            label8.Text = "Zamówienia klienta:";

            // label9
            label9.AutoSize = true;
            label9.Location = new Point(8, 334);
            label9.Name = "label9";
            label9.Text = "Data od:";

            // dateTimePicker1
            dateTimePicker1.Location = new Point(55, 333);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(200, 23);

            // dateTimePicker2
            dateTimePicker2.Location = new Point(307, 330);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(200, 23);

            // label10
            label10.AutoSize = true;
            label10.Location = new Point(260, 336);
            label10.Name = "label10";
            label10.Text = "Data do:";

            // comboBox4
            comboBox4.FormattingEnabled = true;
            comboBox4.Location = new Point(557, 333);
            comboBox4.Name = "comboBox4";
            comboBox4.Size = new Size(121, 23);

            // label11
            label11.AutoSize = true;
            label11.Location = new Point(511, 332);
            label11.Name = "label11";
            label11.Text = "Waluta:";

            // comboBox5
            comboBox5.FormattingEnabled = true;
            comboBox5.Location = new Point(729, 336);
            comboBox5.Name = "comboBox5";
            comboBox5.Size = new Size(121, 23);

            // label12
            label12.AutoSize = true;
            label12.Location = new Point(684, 335);
            label12.Name = "label12";
            label12.Text = "Strona:";

            // listView2
            listView2.Location = new Point(13, 376);
            listView2.Name = "listView2";
            listView2.Size = new Size(1032, 97);

            // comboBox6
            comboBox6.FormattingEnabled = true;
            comboBox6.Location = new Point(924, 336);
            comboBox6.Name = "comboBox6";
            comboBox6.Size = new Size(121, 23);

            // label13
            label13.AutoSize = true;
            label13.Location = new Point(860, 335);
            label13.Name = "label13";
            label13.Text = "Sortuj po:";

            // Form4
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1064, 485);
            Controls.Add(comboBox6);
            Controls.Add(label13);
            Controls.Add(listView2);
            Controls.Add(comboBox5);
            Controls.Add(label12);
            Controls.Add(comboBox4);
            Controls.Add(label11);
            Controls.Add(dateTimePicker2);
            Controls.Add(label10);
            Controls.Add(dateTimePicker1);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(button1);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            Controls.Add(comboBox3);
            Controls.Add(label7);
            Controls.Add(comboBox2);
            Controls.Add(label6);
            Controls.Add(textBox1);
            Controls.Add(label5);
            Controls.Add(comboBox1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(listView1);
            Controls.Add(label1);
            Name = "Form4";
            Text = "Form4";
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Label label1;
        private ListView listView1;
        private Label label2;
        private Label label3;
        private Label label4;
        private ComboBox comboBox1;
        private Label label5;
        private TextBox textBox1;
        private Label label6;
        private ComboBox comboBox2;
        private ComboBox comboBox3;
        private Label label7;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Button button1;
        private Label label8;
        private Label label9;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private Label label10;
        private ComboBox comboBox4;
        private Label label11;
        private ComboBox comboBox5;
        private Label label12;
        private ListView listView2;
        private ComboBox comboBox6;
        private Label label13;
    }
}