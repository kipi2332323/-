using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Data.SQLite;

namespace DrawBoard_self_
{
    public partial class HistoryForm : Form
    {
        private DataGridView dgvHistory;
        private Button btnLoad;
        private Button btnCancel;
        private Form1 _mainForm;

        public HistoryForm(Form1 mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            LoadHistoryList();
        }

        private void InitializeComponent()
        {
            this.Text = "历史绘画列表";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;

            dgvHistory = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            btnLoad = new Button { Text = "加载选中", Dock = DockStyle.Bottom };
            btnCancel = new Button { Text = "取消", Dock = DockStyle.Bottom };

            Panel buttonPanel = new Panel { Height = 40, Dock = DockStyle.Bottom };
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnLoad);
            btnLoad.Location = new Point(buttonPanel.Width - 180, 5);
            btnCancel.Location = new Point(buttonPanel.Width - 90, 5);

            this.Controls.Add(dgvHistory);
            this.Controls.Add(buttonPanel);

            btnLoad.Click += BtnLoad_Click;
            btnCancel.Click += (s, e) => this.Close();
        }

        private void LoadHistoryList()
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                string connStr = $"Data Source={dbPath};Version=3;";
                DataTable dt = new DataTable();
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT Id, Name, CreateTime FROM PaintingHistory ORDER BY CreateTime DESC";
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn))
                    {
                        da.Fill(dt);
                    }
                }
                dgvHistory.DataSource = dt;
                dgvHistory.Columns["Id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载历史列表失败：{ex.Message}");
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (dgvHistory.SelectedRows.Count == 0)
                return;

            int historyId = Convert.ToInt32(dgvHistory.SelectedRows[0].Cells["Id"].Value);
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                string connStr = $"Data Source={dbPath};Version=3;";
                byte[] imageBytes = null;
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT ImageData FROM PaintingHistory WHERE Id = @Id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", historyId);
                        imageBytes = cmd.ExecuteScalar() as byte[];
                    }
                }

                if (imageBytes != null)
                {
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        Bitmap loadedImage = new Bitmap(ms);
                        _mainForm.Invoke(new Action(() => _mainForm.LoadHistoryImage(loadedImage)));
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载历史图片失败：{ex.Message}");
            }
        }
    }
}