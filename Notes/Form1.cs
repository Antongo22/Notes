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
using System.Data.SqlClient;
using System.IO;

namespace Notes
{
    public partial class Form1 : Form
    {
        public static SqlConnection sqlConnection;
        private List<GroupBox> dataGroupBoxes = new List<GroupBox>(); // Список для хранения созданных GroupBox'ов
        private List<GroupBox> notesGroupBoxes = new List<GroupBox>(); // Список для хранения созданных GroupBox'ов для таблицы [Notes]

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

        void LoadDataBase()
        {
            string query = "SELECT [Id], [name], [date], [path] FROM [NotesDate] ORDER BY [date] ASC"; // Сортировка по дате в порядке возрастания
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            int x = 470;
            int y = 30;
            int textBoxWidth = 200;
            int textBoxHeight = 100;

            while (reader.Read())
            {
                int recordId = (int)reader["Id"];
                string name = reader["name"].ToString();
                string path = reader["path"].ToString();
                DateTime date = Convert.ToDateTime(reader["date"]);

                GroupBox groupBox = new GroupBox();
                groupBox.Location = new Point(x, y);
                groupBox.Size = new Size(textBoxWidth + 20, textBoxHeight + 60);
                groupBox.Text = name + "  " + date.ToString();

                // Проверяем, просрочена ли дата
                if (date < DateTime.Now)
                {
                    groupBox.BackColor = Color.Red; // Если просрочено, делаем фон красным
                }

                RichTextBox textBox = new RichTextBox();
                textBox.Location = new Point(10, 20);
                textBox.Size = new Size(textBoxWidth, textBoxHeight);
                textBox.Multiline = true;
                textBox.Font = new Font(textBox.Font.FontFamily, 8); // Шрифт с меньшим размером
                textBox.Text = new Data(path).GetAllText();
                textBox.ReadOnly = true;
                textBox.ScrollBars = RichTextBoxScrollBars.Vertical;

                Button deleteButton = new Button();
                deleteButton.Location = new Point(10, textBoxHeight + 30);
                deleteButton.Size = new Size(textBoxWidth, 30);
                deleteButton.Text = "Удалить";
                deleteButton.Tag = recordId; // Сохраняем Id записи как Tag кнопки
                deleteButton.Click += DeleteButton_Click; // Добавляем обработчик события для кнопки "Удалить"

                groupBox.Controls.Add(textBox);
                groupBox.Controls.Add(deleteButton);
                Controls.Add(groupBox);

                dataGroupBoxes.Add(groupBox); // Добавляем созданный GroupBox в список

                x += groupBox.Width + 10;

                if (x + groupBox.Width > this.Width) // Если выходит за границу формы
                {
                    x = 470; // Начинаем снова с x = 470
                    y += groupBox.Height + 10;
                }
            }

            reader.Close();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (sender is Button deleteButton)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int recordId = (int)deleteButton.Tag; // Получаем Id записи из Tag кнопки
                    string path = GetPathForRecord(recordId); // Получаем путь к файлу для записи

                    // Удаляем запись из базы данных
                    DeleteRecordFromDatabase(recordId);

                    // Удаляем файл
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    // Обновляем интерфейс
                    RefreshInterface();
                }
            }
        }

        private void DeleteRecordFromDatabase(int recordId)
        {
            string deleteQuery = $"DELETE FROM [NotesDate] WHERE [Id] = {recordId}";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);
            deleteCommand.ExecuteNonQuery();
        }

        private string GetPathForRecord(int recordId)
        {
            string query = $"SELECT [path] FROM [NotesDate] WHERE [Id] = {recordId}";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            string path = command.ExecuteScalar()?.ToString();

            if (!string.IsNullOrEmpty(path))
            {
                // Добавляем путь к папке "data"
                path = Path.Combine("data", path);
            }

            return path;
        }

        private void RefreshInterface()
        {
            foreach (var groupBox in dataGroupBoxes)
            {
                // Удаляем GroupBox из формы
                Controls.Remove(groupBox);
            }

            // Очищаем список GroupBox'ов
            dataGroupBoxes.Clear();

            LoadDataBase();
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
            LoadDataBase();
        }

        private void заметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(false);
            form3.Text = "Создание заметки";
            form3.ShowDialog();
            LoadBase();
            LoadDataBase();
        }

        private void заметкуСДатойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(true);
            form3.Text = "Создание заметки с датой";
            form3.ShowDialog();
            LoadBase();
            LoadDataBase();
        }
    }
}
