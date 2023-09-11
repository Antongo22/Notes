using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Notes
{
    public partial class Form1 : Form
    {
        public static SqlConnection sqlConnection;

        void LoadBase()
        {
            string query = "SELECT [name], [path] FROM [Notes]";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            int x = 10;
            int y = 30;
            int textBoxWidth = 200;
            int textBoxHeight = 100;

            while (reader.Read())
            {
                string name = reader["name"].ToString();
                string path = reader["path"].ToString();

                GroupBox groupBox = new GroupBox();
                groupBox.Location = new Point(x, y);
                groupBox.Size = new Size(textBoxWidth + 20, textBoxHeight + 20);
                groupBox.Text = name;

                RichTextBox textBox = new RichTextBox();
                textBox.Location = new Point(10, 20);
                textBox.Size = new Size(textBoxWidth, textBoxHeight);
                textBox.Multiline = true;
                textBox.Font = new Font(textBox.Font.FontFamily, 8); // Шрифт с меньшим размером
                textBox.Text = new Data(path).GetAllText();
                textBox.ReadOnly = true;
                textBox.ScrollBars = RichTextBoxScrollBars.Vertical;

                groupBox.Controls.Add(textBox);
                this.Controls.Add(groupBox);

                x += groupBox.Width + 10;

                if (x + groupBox.Width > 550)
                {
                    x = 10;
                    y += groupBox.Height + 10;
                }
            }

            reader.Close();
        }

        public Form1()
        {
            InitializeComponent();
        }

        ~Form1()
        {
            sqlConnection.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            sqlConnection.Open();
            LoadBase();
        }

        private void заметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(false);
            form3.Text = "Создание заметки";
            form3.ShowDialog();
            LoadBase();
        }

        private void заметкуСДатойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(true);
            form3.Text = "Создание заметки с датой";
            form3.ShowDialog();
            LoadBase();
        }
    }
}
