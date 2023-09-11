using System;
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
        public Form3(bool isDate)
        {
            this.isDate = isDate;
            InitializeComponent();
        }

        private void labelWarning_Click(object sender, EventArgs e)
        {

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
        }
    }
}
