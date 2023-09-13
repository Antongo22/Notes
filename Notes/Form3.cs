using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Notes
{
    public partial class Form3 : Form
    {
        bool isDate;
        bool change;
        int id;

        public Form3(bool isDate, bool change = false, int id = 0)
        {
            this.isDate = isDate;
            this.change = change;
            this.id = id;

            InitializeComponent();
        }
        
        private void Form3_Load(object sender, EventArgs e)
        {
            if (isDate)
            {
                dateTimePicker1.Visible = true;
                dateTimePicker2.Visible = true;

                dateTimePicker1.Format = DateTimePickerFormat.Time;
                dateTimePicker1.ShowUpDown = true;
            }
            if (change)
            {
                buttonSave.Visible = false;
                buttonCh.Visible = true;

                if (isDate)
                {
                    string query = "SELECT [name] FROM [NotesDate] WHERE [Id] = @Id";

                    using (SqlCommand command = new SqlCommand(query, Form1.sqlConnection))
                    {
                        // Добавляем параметр @Id в команду и устанавливаем его значение
                        command.Parameters.AddWithValue("@Id", id);

                        // Выполняем запрос и получаем результат
                        textBoxName.Text = command.ExecuteScalar()?.ToString();
                    }

                    query = "SELECT [path] FROM [NotesDate] WHERE [Id] = @Id";

                    using (SqlCommand command = new SqlCommand(query, Form1.sqlConnection))
                    {
                        // Добавляем параметр @Id в команду и устанавливаем его значение
                        command.Parameters.AddWithValue("@Id", id);

                        // Выполняем запрос и получаем результат
                        string path = command.ExecuteScalar()?.ToString();
                        textBoxText.Text = new Data(path).GetAllText();
                    }


                    query = $"SELECT [date] FROM [NotesDate] WHERE [Id] = {id}";
                    SqlCommand command_ = new SqlCommand(query, Form1.sqlConnection);

                    // Выполнить запрос и получить результат
                    DateTime resultDateTime;

                    object result = command_.ExecuteScalar();

                    resultDateTime = (DateTime)result;

                    dateTimePicker2.Value = resultDateTime.Date; // Устанавливаем только дату
                    dateTimePicker1.Value = resultDateTime; // Устанавливаем дату и время
                }
                else
                {
                    string query = "SELECT [name] FROM [Notes] WHERE [Id] = @Id";

                    using (SqlCommand command = new SqlCommand(query, Form1.sqlConnection))
                    {
                        // Добавляем параметр @Id в команду и устанавливаем его значение
                        command.Parameters.AddWithValue("@Id", id);

                        // Выполняем запрос и получаем результат
                        textBoxName.Text = command.ExecuteScalar()?.ToString();
                    }

                    query = "SELECT [path] FROM [Notes] WHERE [Id] = @Id";

                    using (SqlCommand command = new SqlCommand(query, Form1.sqlConnection))
                    {
                        // Добавляем параметр @Id в команду и устанавливаем его значение
                        command.Parameters.AddWithValue("@Id", id);

                        // Выполняем запрос и получаем результат
                        string path = command.ExecuteScalar()?.ToString();
                        textBoxText.Text = new Data(path).GetAllText();
                    }

                }

            }
        }


        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) && !string.IsNullOrEmpty(textBoxText.Text) && !isDate)
            {
                // SQL-запрос INSERT
                string insertQuery = $"INSERT INTO Notes (name, path) VALUES (N'{textBoxName.Text}', N''); SELECT SCOPE_IDENTITY();";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, Form1.sqlConnection))
                {
                    try
                    {
                        // Выполняем вставку и получаем сгенерированный Id
                        int insertedId = Convert.ToInt32(insertCommand.ExecuteScalar());

                        if (insertedId > 0)
                        {
                            // Обновляем path с учетом Id
                            string updatedPath = $"note{insertedId}.txt";
                            string updatePathQuery = $"UPDATE Notes SET path = N'{updatedPath}' WHERE Id = {insertedId}";

                            using (SqlCommand updateCommand = new SqlCommand(updatePathQuery, Form1.sqlConnection))
                            {
                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Запись успешно добавлена в таблицу.");
                                    new Data(updatedPath).Add(textBoxText.Text);
                                    Close();
                                }
                                else
                                {
                                    MessageBox.Show("Произошла ошибка при обновлении пути записи.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при добавлении записи.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка: " + ex.Message);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(textBoxName.Text) && !string.IsNullOrEmpty(textBoxText.Text) && isDate)
            {
                DateTime date1 = dateTimePicker2.Value;
                DateTime date2 = dateTimePicker1.Value;

                DateTime combinedDateTime = new DateTime(date1.Year, date1.Month, date1.Day, date2.Hour, date2.Minute, date2.Minute);

                if (combinedDateTime < DateTime.Now)
                {
                    MessageBox.Show("Нельзя вводить дату и время, которые уже прошли!");
                    return;
                }

                // SQL-запрос INSERT
                string insertQuery = $"INSERT INTO NotesDate (name, [date], path) VALUES (N'{textBoxName.Text}', '{combinedDateTime.ToString("yyyy-MM-dd HH:mm:ss")}', N'')";

                using (SqlCommand insertCommand = new SqlCommand(insertQuery, Form1.sqlConnection))
                {
                    try
                    {
                        // Выполняем вставку
                        int rowsAffected = insertCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Получаем айди последней записи
                            string getLastIdQuery = "SELECT TOP 1 Id FROM NotesDate ORDER BY Id DESC";

                            using (SqlCommand getLastIdCommand = new SqlCommand(getLastIdQuery, Form1.sqlConnection))
                            {
                                int lastInsertedId = (int)getLastIdCommand.ExecuteScalar();

                                // Обновляем path с учетом Id
                                string updatedPath = $"noteDate{lastInsertedId}.txt";
                                string updatePathQuery = $"UPDATE NotesDate SET path = N'{updatedPath}' WHERE Id = {lastInsertedId}";

                                using (SqlCommand updateCommand = new SqlCommand(updatePathQuery, Form1.sqlConnection))
                                {
                                    int updateRowsAffected = updateCommand.ExecuteNonQuery();

                                    if (updateRowsAffected > 0)
                                    {
                                        MessageBox.Show("Запись успешно добавлена в таблицу.");
                                        new Data(updatedPath).Add(textBoxText.Text);
                                        Close();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Произошла ошибка при обновлении пути записи.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при добавлении записи.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Заполнитевсе данные!");
            }
        }

        private void buttonCh_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) && !string.IsNullOrEmpty(textBoxText.Text) && !isDate)
            {
                string updateQuery = $"UPDATE [Notes] SET [name] = N'{textBoxName.Text}' WHERE [Id] = {id}";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, Form1.sqlConnection))
                {
                    try
                    {
                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Запрос для получения path по id
                            string pathQuery = $"SELECT [path] FROM [Notes] WHERE [Id] = {id}";

                            using (SqlCommand pathCommand = new SqlCommand(pathQuery, Form1.sqlConnection))
                            {
                                try
                                {
                                    string path = pathCommand.ExecuteScalar() as string;


                                    // Очистить содержимое файла по указанному path
                                    new Data(path).Clear();

                                    // Добавить новое содержимое из textBoxText.Text
                                    new Data(path).Add(textBoxText.Text);

                                    MessageBox.Show("Запись успешно обновлена в таблице и файле.");
                                    Close();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Произошла ошибка при обновлении файла: " + ex.Message);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Запись с указанным id не найдена.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка при обновлении записи: " + ex.Message);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(textBoxName.Text) && !string.IsNullOrEmpty(textBoxText.Text) && isDate)
            {
                string newName = textBoxName.Text;
                DateTime date1 = dateTimePicker2.Value;
                DateTime date2 = dateTimePicker1.Value;

                DateTime combinedDateTime = new DateTime(date1.Year, date1.Month, date1.Day, date2.Hour, date2.Minute, date2.Second);

                if (combinedDateTime < DateTime.Now)
                {
                    MessageBox.Show("Нельзя вводить дату и время, которые уже прошли!");
                    return;
                }

                try
                {
                    // Обновление данных в базе данных
                    string updateQuery = "UPDATE NotesDate SET name = @name, date = @date WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(updateQuery, Form1.sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@name", newName);
                        cmd.Parameters.AddWithValue("@date", combinedDateTime);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Получение path по id
                    string selectPathQuery = "SELECT path FROM NotesDate WHERE id = @id";
                    string path = string.Empty;
                    using (SqlCommand cmd = new SqlCommand(selectPathQuery, Form1.sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            path = reader["path"].ToString();
                            reader.Close(); // Закройте DataReader после использования его данных

                            // Очистка содержимого файла по указанному path
                            new Data(path).Clear();

                            // Добавление нового содержимого из textBoxText.Text
                            new Data(path).Add(textBoxText.Text);
                        }
                    }

                    MessageBox.Show("Запись успешно обновлена в таблице и файле.");
                    Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Заполнитевсе данные!");
            }
        }
    }
    
}
