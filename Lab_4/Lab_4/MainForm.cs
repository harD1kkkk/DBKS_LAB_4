using Microsoft.Data.SqlClient;
using System.Data;

namespace Lab_4
{
    public class MainForm : Form
    {
        private readonly TextBox txtTitle = new();
        private readonly TextBox txtText = new()
        {
            Multiline = true,
            AcceptsReturn = true,
            ScrollBars = ScrollBars.Vertical,
            WordWrap = true
        };
        private readonly PictureBox pictureBox = new();
        private readonly DataGridView grid = new();

        private byte[]? imageData;

        public MainForm()
        {
            Text = "Lab_4 — Книги";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1000;
            Height = 700;
            MinimumSize = new Size(900, 600);

            BuildUi();
            LoadBooks();
        }

        private void BuildUi()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };

            mainPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50)
            );

            mainPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50)
            );

            var leftPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 7,
                ColumnCount = 1
            };

            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblTitle = new Label
            {
                Text = "Назва книги:",
                AutoSize = true,
                Padding = new Padding(0, 5, 0, 5)
            };

            txtTitle.Dock = DockStyle.Fill;

            var lblText = new Label
            {
                Text = "Текстова примітка:",
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
            };

            txtText.Dock = DockStyle.Fill;
            txtText.ForeColor = SystemColors.WindowText;
            txtText.BackColor = SystemColors.Window;

            var lblImage = new Label
            {
                Text = "Зображення:",
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 5)
            };

            pictureBox.Dock = DockStyle.Fill;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.MinimumSize = new Size(200, 200);

            var btnLoadImage = new Button
            {
                Text = "Завантажити зображення",
                AutoSize = true
            };

            btnLoadImage.Click += (_, _) => LoadImage();

            var btnAdd = new Button
            {
                Text = "Додати",
                AutoSize = true
            };

            btnAdd.Click += (_, _) => AddBook();

            leftPanel.Controls.Add(lblTitle, 0, 0);
            leftPanel.Controls.Add(txtTitle, 0, 1);
            leftPanel.Controls.Add(lblText, 0, 2);
            leftPanel.Controls.Add(txtText, 0, 3);
            leftPanel.Controls.Add(lblImage, 0, 4);
            leftPanel.Controls.Add(pictureBox, 0, 5);

            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            buttonsPanel.Controls.Add(btnLoadImage);
            buttonsPanel.Controls.Add(btnAdd);

            leftPanel.Controls.Add(buttonsPanel, 0, 6);

            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AutoGenerateColumns = false;

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id"
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Title",
                HeaderText = "Назва",
                DataPropertyName = "Title"
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Text",
                HeaderText = "Примітка",
                DataPropertyName = "Text"
            });

            grid.CellClick += Grid_CellClick;

            mainPanel.Controls.Add(leftPanel, 0, 0);
            mainPanel.Controls.Add(grid, 1, 0);

            Controls.Add(mainPanel);
        }

        private void LoadBooks()
        {
            const string sql = """
            SELECT Id, Title, Text, ImageData
            FROM dbo.Books
            ORDER BY Id;
            """;

            try
            {
                using var connection =
                    new SqlConnection(DbConfig.ConnectionString);

                using var command =
                    new SqlCommand(sql, connection);

                using var adapter =
                    new SqlDataAdapter(command);

                var table = new DataTable();
                adapter.Fill(table);

                grid.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка завантаження даних:\n{ex.Message}",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadImage()
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Зображення|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            imageData = File.ReadAllBytes(dialog.FileName);

            using var stream =
                new MemoryStream(imageData);

            using var tempImage =
                Image.FromStream(stream);

            pictureBox.Image = new Bitmap(tempImage);
        }

        private void AddBook()
        {
            string title = txtTitle.Text.Trim();
            string text = txtText.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show(
                    "Введіть назву книги.",
                    "Перевірка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(
                    "Введіть текстову примітку.",
                    "Перевірка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            const string sql = """
            INSERT INTO dbo.Books
                (Title, Text, ImageData)
            VALUES
                (@Title, @Text, @ImageData);
            """;

            try
            {
                using var connection =
                    new SqlConnection(DbConfig.ConnectionString);

                using var command =
                    new SqlCommand(sql, connection);

                command.Parameters.Add(
                    new SqlParameter("@Title", SqlDbType.NVarChar, 100)
                    {
                        Value = title
                    }
                );

                command.Parameters.Add(
                    new SqlParameter("@Text", SqlDbType.NVarChar, -1)
                    {
                        Value = text
                    }
                );

                command.Parameters.Add(
                    new SqlParameter("@ImageData", SqlDbType.VarBinary, -1)
                    {
                        Value = imageData ?? (object)DBNull.Value
                    }
                );

                connection.Open();
                command.ExecuteNonQuery();

                MessageBox.Show(
                    "Дані успішно додано.",
                    "Готово",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ClearInputs();
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Помилка додавання:\n{ex.Message}",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void Grid_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var row = grid.Rows[e.RowIndex];

            txtTitle.Text =
                row.Cells["Title"].Value?.ToString() ?? "";

            txtText.Text =
                row.Cells["Text"].Value?.ToString() ?? "";

            if (row.DataBoundItem is DataRowView dataRow)
            {
                object value = dataRow["ImageData"];

                if (value != DBNull.Value &&
                    value is byte[] bytes)
                {
                    imageData = bytes;

                    using var stream =
                        new MemoryStream(bytes);

                    using var tempImage =
                        Image.FromStream(stream);

                    pictureBox.Image =
                        new Bitmap(tempImage);
                }
                else
                {
                    imageData = null;
                    pictureBox.Image = null;
                }
            }
        }

        private void ClearInputs()
        {
            txtTitle.Clear();
            txtText.Clear();
            imageData = null;
            pictureBox.Image = null;
        }
    }
}
