using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace _6_model_2_zd
{
    public partial class Form1 : Form
    {
        SqlConnection connection = new SqlConnection(@"Data Source=Dmitry; Initial Catalog=TaskAccounting; Integrated Security=True");
        SqlDataAdapter adapter;
        DataTable tasksTable;
        bool isEditing = false; // Флаг для отслеживания режима редактирования
        int editedTaskId = -1; // Идентификатор редактируемой задачи

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализируем DataGridView
            tasksTable = new DataTable();
            dataGridView1.DataSource = tasksTable;

            // Загрузим данные из базы данных при запуске приложения
            LoadTasksFromDatabase();
        }

        private void LoadTasksFromDatabase()
        {
            using (adapter = new SqlDataAdapter("SELECT id AS 'ID', title AS 'Задача', status AS 'Статус' FROM tasks", connection))
            {

                tasksTable.Clear();
                adapter.Fill(tasksTable);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1) // Проверка на выбор одной строки для удаления
            {
                int taskId = (int)dataGridView1.SelectedRows[0].Cells["ID"].Value;

                // Удаление задачи из базы данных
                using (SqlCommand cmd = new SqlCommand("DELETE FROM tasks WHERE id = @id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", taskId);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                // Обновление DataGridView
                LoadTasksFromDatabase();
            }
            else if (dataGridView1.SelectedRows.Count > 1) // Если выбрано более одной строки
            {
                MessageBox.Show("Нельзя выбрать несколько элементов для удаления. Пожалуйста, выберите только одну строку.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isEditing) // Если режим редактирования включен
            {
                // Сохранить изменения в базе данных
                string taskTitle = textBox1.Text;
                string taskStatus = comboBox1.Text;

                // Проверка, что TextBox не пуст
                if (string.IsNullOrWhiteSpace(taskTitle))
                {
                    MessageBox.Show("Заполните поле Задача", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Прерываем выполнение метода
                }

                // Проверка, что ComboBox выбран
                if (string.IsNullOrWhiteSpace(taskStatus))
                {
                    MessageBox.Show("Выберите Статус.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Прерываем выполнение метода
                }

                // Обновление задачи в базе данных
                using (SqlCommand cmd = new SqlCommand("UPDATE tasks SET title = @title, status = @status WHERE id = @id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", editedTaskId);
                    cmd.Parameters.AddWithValue("@title", taskTitle);
                    cmd.Parameters.AddWithValue("@status", taskStatus);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                isEditing = false; // Отключить режим редактирования
                editedTaskId = -1;

                // Обновление DataGridView
                LoadTasksFromDatabase();
            }
            else
            {
                string taskTitle = textBox1.Text;
                string taskStatus = comboBox1.Text;

                // Проверка, что TextBox не пуст
                if (string.IsNullOrWhiteSpace(taskTitle))
                {
                    MessageBox.Show("Заполните поле 'Задача'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Прерываем выполнение метода
                }

                // Проверка, что ComboBox выбран
                if (string.IsNullOrWhiteSpace(taskStatus))
                {
                    MessageBox.Show("Выберите 'Статус'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Прерываем выполнение метода
                }

                // Вставка новой задачи в базу данных
                using (SqlCommand cmd = new SqlCommand("INSERT INTO tasks (title, status) VALUES (@title, @status)", connection))
                {
                    cmd.Parameters.AddWithValue("@title", taskTitle);
                    cmd.Parameters.AddWithValue("@status", taskStatus);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                // Обновление DataGridView
                LoadTasksFromDatabase();

                // Очистка полей ввода
                textBox1.Clear();
                comboBox1.SelectedIndex = -1;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                isEditing = true; // Включить режим редактирования
                editedTaskId = (int)dataGridView1.Rows[e.RowIndex].Cells["ID"].Value;
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Задача"].Value.ToString();
                comboBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Статус"].Value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Предупреждение перед удалением всех данных
            DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить все задачи?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Удаление всех задач из базы данных
                using (SqlCommand cmd = new SqlCommand("DELETE FROM tasks", connection))
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }

                // Обновление DataGridView после удаления
                LoadTasksFromDatabase();
            }
        }
    }
}
