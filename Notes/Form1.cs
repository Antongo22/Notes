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
        List<GroupBox> dataGroupBoxes = new List<GroupBox>(); // Список для хранения созданных GroupBox'ов
        List<GroupBox> notesGroupBoxes = new List<GroupBox>(); // Список для хранения созданных GroupBox'ов для таблицы [Notes]
        Timer dataLoadTimer;

        #region Вывод заметок
        /// <summary>
        /// Загрузка всех обычных заметок 
        /// </summary>
        void LoadBase()
        {
            string query = "SELECT [Id], [name], [path] FROM [Notes]";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            int x = 10;
            int y = 30;
            int textBoxWidth = 200;
            int textBoxHeight = 100;

            while (reader.Read())
            {
                int recordId = (int)reader["Id"];
                string name = reader["name"].ToString();
                string path = reader["path"].ToString();

                GroupBox groupBox = new GroupBox();
                groupBox.Location = new Point(x, y);
                groupBox.Size = new Size(textBoxWidth + 20, textBoxHeight + 65);
                groupBox.Text = name.Length > 25 ? name.Substring(0, 22) + "..." : name;

                RichTextBox textBox = new RichTextBox();
                textBox.Location = new Point(10, 20);
                textBox.Size = new Size(textBoxWidth, textBoxHeight);
                textBox.Multiline = true;
                textBox.Font = new Font(textBox.Font.FontFamily, 8); 
                textBox.Text = new Data(path).GetAllText();
                textBox.ReadOnly = true;
                textBox.ScrollBars = RichTextBoxScrollBars.Vertical;

                Button deleteButton = new Button();
                deleteButton.Location = new Point(textBoxWidth / 2 + 15, textBoxHeight + 30);
                deleteButton.Size = new Size(textBoxWidth / 2 - 10, 30);
                deleteButton.Text = "Удалить";
                deleteButton.Tag = recordId; 
                deleteButton.Click += DeleteButtonNotes;

                Button change = new Button();
                change.Location = new Point(15, textBoxHeight + 30);
                change.Size = new Size(textBoxWidth / 2 - 10, 30);
                change.Text = "Изменить";
                change.Tag = recordId; 
                change.Click += (sender, e) =>
                {
                    Change(false, recordId);
                };

                groupBox.Controls.Add(textBox);
                groupBox.Controls.Add(deleteButton);
                groupBox.Controls.Add(change);
                this.Controls.Add(groupBox);

                notesGroupBoxes.Add(groupBox);

                x += groupBox.Width + 10;

                if (x + groupBox.Width > 550)
                {
                    x = 10;
                    y += groupBox.Height + 10;
                }
            }

            reader.Close();
        }

        /// <summary>
        /// Загрузка всех заметок c датой
        /// </summary>
        void LoadDataBase()
        {
            DateTime startDate = dateTimePicker1.Value;
            DateTime endDate = dateTimePicker2.Value;

            string query = "SELECT [Id], [name], [date], [path] FROM [NotesDate] WHERE [date] BETWEEN @StartDate AND @EndDate ORDER BY [date] ASC";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@StartDate", startDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            SqlDataReader reader = command.ExecuteReader();

            int x = 470;
            int y = 60;
            int textBoxWidth = 205;
            int textBoxHeight = 100;

            while (reader.Read())
            {
                int recordId = (int)reader["Id"];
                string name = reader["name"].ToString();
                string path = reader["path"].ToString();
                DateTime date = Convert.ToDateTime(reader["date"]);

                GroupBox groupBox = new GroupBox();
                groupBox.Location = new Point(x, y);
                groupBox.Size = new Size(textBoxWidth + 20, textBoxHeight + 65);
                groupBox.Text = groupBox.Text = (name.Length > 15 ? name.Substring(0, 12) + "..." : name + "  ") + date.ToString();

                if (date < DateTime.Now)
                {
                    groupBox.BackColor = Color.Red; 
                }

                RichTextBox textBox = new RichTextBox();
                textBox.Location = new Point(10, 20);
                textBox.Size = new Size(textBoxWidth, textBoxHeight);
                textBox.Multiline = true;
                textBox.Font = new Font(textBox.Font.FontFamily, 8); 
                textBox.Text = new Data(path).GetAllText();
                textBox.ReadOnly = true;
                textBox.ScrollBars = RichTextBoxScrollBars.Vertical;

                Button deleteButton = new Button();
                deleteButton.Location = new Point(textBoxWidth / 2 + 15, textBoxHeight + 30);
                deleteButton.Size = new Size(textBoxWidth / 2 - 10, 30);
                deleteButton.Text = "Удалить";
                deleteButton.Tag = recordId;
                deleteButton.Click += DeleteButtonNotesDate; 

                Button change = new Button();
                change.Location = new Point(15, textBoxHeight + 30);
                change.Size = new Size(textBoxWidth / 2 - 10, 30);
                change.Text = "Изменить";
                change.Tag = recordId; 
                change.Click += (sender, e) =>
                {
                    Change(true, recordId); 
                };

                groupBox.Controls.Add(textBox);
                groupBox.Controls.Add(deleteButton);
                groupBox.Controls.Add(change);
                Controls.Add(groupBox);

                dataGroupBoxes.Add(groupBox); 

                x += groupBox.Width + 10;

                if (x + groupBox.Width > this.Width) 
                {
                    x = 470; 
                    y += groupBox.Height + 10;
                }
            }

            reader.Close();
        }

        #region методы LoadBase

        /// <summary>
        /// Кнопка удаления заметки для обычных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButtonNotes(object sender, EventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.Tag is int recordId)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Удаление файла
                    DeleteFileNotes(recordId);

                    // Удаление записи из базы данных
                    DeleteDatabaseNotes(recordId);

                    // Обновление интерфейса
                    RefreshNotes();
                }
            }
        }

        /// <summary>
        /// Удаление записи из базы
        /// </summary>
        /// <param name="recordId"></param>
        private void DeleteDatabaseNotes(int recordId)
        {
            string deleteQuery = $"DELETE FROM [Notes] WHERE [Id] = @RecordId";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);
            deleteCommand.Parameters.AddWithValue("@RecordId", recordId);
            deleteCommand.ExecuteNonQuery();
        }

        private void DeleteFileNotes(int recordId)
        {
            string query = $"SELECT [path] FROM [Notes] WHERE [Id] = @RecordId";
            SqlCommand command = new SqlCommand(query, sqlConnection);
            command.Parameters.AddWithValue("@RecordId", recordId);
            string path = command.ExecuteScalar()?.ToString();

            if (!string.IsNullOrEmpty(path))
            {
                string filePath = Path.Combine("data", path);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// Перезагрузка интерфейса записей
        /// </summary>
        private void RefreshNotes()
        {
            foreach (var groupBox in notesGroupBoxes)
            {
                // Удаляем GroupBox из формы
                Controls.Remove(groupBox);
            }

            // Очищаем список GroupBox'ов
            notesGroupBoxes.Clear();

            LoadBase();
        }

        #endregion

        #region методы LoadDataBase
        /// <summary>
        /// Кнопка удаления заметок с датой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DeleteButtonNotesDate(object sender, EventArgs e)
        {
            if (sender is Button deleteButton)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int recordId = (int)deleteButton.Tag; // Получаем Id записи из Tag кнопки
                    string path = GetPathNotesDate(recordId); // Получаем путь к файлу для записи

                    // Удаляем запись из базы данных
                    DeleteDatabaseNotesDate(recordId);

                    // Удаляем файл
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    // Обновляем интерфейс
                    RefreshNotesDate();
                }
            }
        }

        /// <summary>
        /// Удаление записей в таблице
        /// </summary>
        /// <param name="recordId"></param>
        void DeleteDatabaseNotesDate(int recordId)
        {
            string deleteQuery = $"DELETE FROM [NotesDate] WHERE [Id] = {recordId}";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);
            deleteCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Поучение пути файла с заметкой
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        string GetPathNotesDate(int recordId)
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

        /// <summary>
        /// Перезагрузка интерфейса
        /// </summary>
        void RefreshNotesDate()
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

        #endregion

        /// <summary>
        /// Изминение заметки
        /// </summary>
        /// <param name="isDate"></param>
        /// <param name="id"></param>
        void Change(bool isDate, int id)
        {
            Form3 form3 = new Form3(isDate, true, id);
            form3.Text = "Изминение заметки";
            form3.ShowDialog();
            RefreshNotesDate();
            RefreshNotes();
            LoadBase();
            LoadDataBase();
        }

        #endregion

        #region Инициализация формыы
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
            dateTimePicker1.Value = DateTimePicker.MinimumDateTime; // Устанавливаем минимальную дату
            dateTimePicker2.Value = DateTimePicker.MaximumDateTime; // Устанавливаем максимальную дату

            // Инициализация таймера
            dataLoadTimer = new Timer();
            dataLoadTimer.Interval = 60000; // Интервал в миллисекундах (1 минута)
            dataLoadTimer.Tick += DataLoadTimer_Tick;
            dataLoadTimer.Start(); // Запуск таймера

            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Data"].ConnectionString);
            sqlConnection.Open();
            LoadBase();
            LoadDataBase();
            CheckAndShowExpiredRecords();
        }

        #endregion

        #region Обновление данных на форме
        /// <summary>
        /// Обновление данных о заметках
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataLoadTimer_Tick(object sender, EventArgs e)
        {
            RefreshNotesDate();
            LoadDataBase();
            CheckAndShowExpiredRecords();
        }

        /// <summary>
        /// Проверка того, что заметки просрочены
        /// </summary>
        private void CheckAndShowExpiredRecords()
        {
            string selectQuery = "SELECT COUNT(*) FROM [NotesDate] WHERE [date] < GETDATE()";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            int expiredRecordCount = (int)selectCommand.ExecuteScalar();

            if (expiredRecordCount > 0)
            {
                MessageBox.Show("Обнаружены просроченные записи.", "Просроченные записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Верхняя панель
        private void заметкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(false);
            form3.Text = "Создание заметки";
            form3.ShowDialog();
            RefreshNotesDate();
            RefreshNotes();
            LoadBase();
            LoadDataBase();
        }

        private void заметкуСДатойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(true);
            form3.Text = "Создание заметки с датой";
            form3.ShowDialog();
            RefreshNotesDate();
            RefreshNotes();
            LoadBase();
            LoadDataBase();
        }

        private void удалитьПросроченныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить просроченные записи?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteExpiredRecords();
                RefreshNotesDate(); // Обновите интерфейс после удаления
                LoadDataBase(); // Загрузите данные снова после удаления
            }
        }

        private void DeleteExpiredRecords()
        {
            string selectQuery = "SELECT [Id], [path] FROM [NotesDate] WHERE [date] < GETDATE()";
            SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
            SqlDataReader reader = selectCommand.ExecuteReader();

            // Создаем список для хранения ID записей и связанных с ними путей к файлам
            List<Tuple<int, string>> recordsToDelete = new List<Tuple<int, string>>();

            while (reader.Read())
            {
                int recordId = (int)reader["Id"];
                string filePath = Path.Combine("data", reader["path"].ToString());

                recordsToDelete.Add(Tuple.Create(recordId, filePath));
            }

            reader.Close();

            // Удаление записей и связанных файлов
            foreach (var recordToDelete in recordsToDelete)
            {
                int recordId = recordToDelete.Item1;
                string filePath = recordToDelete.Item2;

                // Удаляем запись из базы данных
                DeleteDatabaseNotesDate(recordId);

                // Удаляем файл, если он существует
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
        #endregion

        private void сбросДатыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTimePicker.MinimumDateTime; // Устанавливаем минимальную дату

            dateTimePicker2.Value = DateTimePicker.MaximumDateTime; // Устанавливаем максимальную дату

            LoadDataBase();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadDataBase();
        }
    }
}
