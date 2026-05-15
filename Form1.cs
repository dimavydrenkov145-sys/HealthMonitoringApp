using System;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace HealthMonitoringApp
{
    public partial class Form1 : Form
    {
        // База данных
        private string connectionString = "Data Source=health.db;Version=3;";

        // Элементы управления (объявляем вручную)
        private GroupBox groupBoxInput;
        private Label lblSteps, lblCalories, lblPulse, lblPressure, lblSugar;
        private TextBox txtSteps, txtCalories, txtPulse, txtSystolic, txtDiastolic, txtSugar;
        private Button btnSave;
        private Label lblRecommendation;
        private GroupBox groupBoxHistory;
        private DataGridView dataGridViewHistory;

        public Form1()
        {
            // Создаём интерфейс программно
            InitializeComponentManual();

            // Создаём БД и загружаем историю
            CreateDatabase();
            LoadHistory();
        }

        /// <summary>
        /// Ручное создание всех контролов и размещение их на форме
        /// </summary>
        private void InitializeComponentManual()
        {
            // Настройки формы
            this.Text = "Сервис мониторинга здоровья";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ========== GroupBox для ввода данных ==========
            groupBoxInput = new GroupBox()
            {
                Text = "Ввод показателей здоровья",
                Location = new Point(12, 12),
                Size = new Size(400, 280)
            };

            // Метки и поля ввода
            lblSteps = new Label() { Text = "Шаги:", Location = new Point(10, 30), Size = new Size(80, 25) };
            txtSteps = new TextBox() { Location = new Point(120, 30), Size = new Size(150, 25) };

            lblCalories = new Label() { Text = "Калории:", Location = new Point(10, 65), Size = new Size(80, 25) };
            txtCalories = new TextBox() { Location = new Point(120, 65), Size = new Size(150, 25) };

            lblPulse = new Label() { Text = "Пульс (уд/мин):", Location = new Point(10, 100), Size = new Size(100, 25) };
            txtPulse = new TextBox() { Location = new Point(120, 100), Size = new Size(150, 25) };

            lblPressure = new Label() { Text = "Давление:", Location = new Point(10, 135), Size = new Size(80, 25) };
            txtSystolic = new TextBox() { Location = new Point(120, 135), Size = new Size(60, 25) };
            Label lblSlash = new Label() { Text = "/", Location = new Point(185, 135), Size = new Size(10, 25) };
            txtDiastolic = new TextBox() { Location = new Point(200, 135), Size = new Size(60, 25) };

            lblSugar = new Label() { Text = "Уровень сахара (ммоль/л):", Location = new Point(10, 170), Size = new Size(150, 25) };
            txtSugar = new TextBox() { Location = new Point(160, 170), Size = new Size(100, 25) };

            btnSave = new Button()
            {
                Text = "Сохранить показатель",
                Location = new Point(120, 210),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen
            };
            btnSave.Click += btnSave_Click;

            // Добавляем все элементы в groupBoxInput
            groupBoxInput.Controls.AddRange(new Control[] {
                lblSteps, txtSteps,
                lblCalories, txtCalories,
                lblPulse, txtPulse,
                lblPressure, txtSystolic, lblSlash, txtDiastolic,
                lblSugar, txtSugar,
                btnSave
            });

            // ========== Метка для рекомендаций ==========
            lblRecommendation = new Label()
            {
                Location = new Point(12, 300),
                Size = new Size(760, 60),
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Text = "Рекомендация: введите данные и нажмите 'Сохранить'"
            };

            // ========== GroupBox для истории ==========
            groupBoxHistory = new GroupBox()
            {
                Text = "История измерений (последние 20 записей)",
                Location = new Point(12, 370),
                Size = new Size(760, 180)
            };

            dataGridViewHistory = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            groupBoxHistory.Controls.Add(dataGridViewHistory);

            // Добавляем всё на форму
            this.Controls.AddRange(new Control[] {
                groupBoxInput,
                lblRecommendation,
                groupBoxHistory
            });
        }

        /// <summary>
        /// Создание таблицы в БД (если не существует)
        /// </summary>
        private void CreateDatabase()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS HealthRecords (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                Date TEXT,
                                Steps INTEGER,
                                Calories INTEGER,
                                Pulse INTEGER,
                                Systolic INTEGER,
                                Diastolic INTEGER,
                                BloodSugar REAL)";
                using (var cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Сохранение показателей в БД
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверка на пустые поля
                if (string.IsNullOrWhiteSpace(txtSteps.Text) ||
                    string.IsNullOrWhiteSpace(txtCalories.Text) ||
                    string.IsNullOrWhiteSpace(txtPulse.Text) ||
                    string.IsNullOrWhiteSpace(txtSystolic.Text) ||
                    string.IsNullOrWhiteSpace(txtDiastolic.Text) ||
                    string.IsNullOrWhiteSpace(txtSugar.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Предупреждение",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int steps = int.Parse(txtSteps.Text);
                int calories = int.Parse(txtCalories.Text);
                int pulse = int.Parse(txtPulse.Text);
                int systolic = int.Parse(txtSystolic.Text);
                int diastolic = int.Parse(txtDiastolic.Text);

                // Используем InvariantCulture для поддержки точки как разделителя
                double sugar = double.Parse(txtSugar.Text, System.Globalization.CultureInfo.InvariantCulture);

                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"INSERT INTO HealthRecords (Date, Steps, Calories, Pulse, Systolic, Diastolic, BloodSugar)
                           VALUES (@date, @steps, @cal, @pulse, @sys, @dias, @sugar)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@steps", steps);
                        cmd.Parameters.AddWithValue("@cal", calories);
                        cmd.Parameters.AddWithValue("@pulse", pulse);
                        cmd.Parameters.AddWithValue("@sys", systolic);
                        cmd.Parameters.AddWithValue("@dias", diastolic);
                        cmd.Parameters.AddWithValue("@sugar", sugar);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadHistory();
                ShowRecommendation(systolic, pulse, sugar);
                ClearInputFields();
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Ошибка формата данных. Убедитесь, что:\n" +
                                "- Шаги, калории, пульс, давление — целые числа\n" +
                                "- Сахар введён числом (можно с точкой, например 5.5)\n\n" +
                                "Подробнее: " + ex.Message, "Ошибка ввода",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка истории в DataGridView
        /// </summary>
        private void LoadHistory()
        {
            var dt = new System.Data.DataTable();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Date as 'Дата', Steps as 'Шаги', Calories as 'Калории', Pulse as 'Пульс', " +
                             "Systolic || '/' || Diastolic as 'Давление', BloodSugar as 'Сахар' " +
                             "FROM HealthRecords ORDER BY Date DESC LIMIT 20";
                using (var adapter = new SQLiteDataAdapter(sql, conn))
                {
                    adapter.Fill(dt);
                }
            }
            dataGridViewHistory.DataSource = dt;
        }

        /// <summary>
        /// Формирование текстовой рекомендации
        /// </summary>
        private void ShowRecommendation(int systolic, int pulse, double sugar)
        {
            string rec = "";
            if (systolic > 140)
                rec += "⚠️ Повышенное давление. Обратитесь к врачу. ";
            else if (systolic < 100)
                rec += "🔻 Низкое давление. Отдохните, выпейте воды. ";

            if (pulse > 100)
                rec += "❤️ Учащённый пульс. Избегайте стресса. ";
            else if (pulse < 60)
                rec += "🐢 Замедленный пульс. Проконсультируйтесь с врачом. ";

            if (sugar > 6.1)
                rec += "🍬 Повышенный сахар крови. Рекомендуется диета. ";

            if (string.IsNullOrEmpty(rec))
                rec = "✅ Все показатели в норме. Отличная работа!";

            lblRecommendation.Text = rec;
        }

        /// <summary>
        /// Очистка полей ввода
        /// </summary>
        private void ClearInputFields()
        {
            txtSteps.Text = "";
            txtCalories.Text = "";
            txtPulse.Text = "";
            txtSystolic.Text = "";
            txtDiastolic.Text = "";
            txtSugar.Text = "";
        }
    }
}