using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Notes
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void labelWarning_Click(object sender, EventArgs e)
        {

        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxName.Text) && !string.IsNullOrEmpty(textBoxText.Text))
            {               
                try
                {
                    // Form1.sqlConnection.Open();

                    // Создание пути с использованием текущего id 
                    int id = ++Data.id; 
                    string path = $"note{id}.txt";

                    // SQL-запрос INSERT
                    string insertQuery = $"INSERT INTO Notes (name, path) VALUES (N'{textBoxName.Text}', N'{path}')";

                    using (SqlCommand command = new SqlCommand(insertQuery, Form1.sqlConnection))
                    {

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Запись успешно добавлена в таблицу.");
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при добавлении записи.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                    throw ex;
                }
                finally
                {

                    Form1.sqlConnection.Close();
                }
            }
            else
            {
                labelWarning.Visible = true;
            }
        }
    }
}
