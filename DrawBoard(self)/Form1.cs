using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;           // 需要安装 Newtonsoft.Json NuGet 包

namespace DrawBoard_self_
{
    public partial class Form1 : Form
    {
        private bool _isServerMode = false;
        public bool IsServerMode => _isServerMode;

        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;
        private DrawingTool _currentTool;
        private Color _penColor = Color.Black;
        private int _penWidth = 2;
        private bool _isPreviewing = false;
        private Point _previewStart;
        private Point _previewEnd;
        private PreviewShape _previewShape = PreviewShape.None;
        private readonly string _currentRoomId = "Room1001";
        private readonly DrawClient _drawClient;
        private readonly DrawServer _drawServer;

        // 保存菜单中的 IP 输入框引用
        private TextBox _menuIpTextBox;

        public enum PreviewShape
        {
            None, Line, Rectangle, Ellipse, Circle
        }

        public Graphics Graphics => _graphics;
        public Color PenColor => _penColor;
        public int PenWidth => _penWidth;
        public bool IsDrawing { get; set; }
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public PictureBox Canvas => canvas;
        public string CurrentRoomId => _currentRoomId;

        private int? _currentHistoryId = null;   // 当前画板对应的历史记录ID（如果存在）

        public Form1()
        {
            InitializeComponent();

            InitSQLiteDb();

            _drawClient = new DrawClient(this);
            _drawServer = new DrawServer(this);

            _bitmap = new Bitmap(canvas.Width, canvas.Height);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.Clear(Color.White);
            canvas.Image = _bitmap;

            _currentTool = new CurveTool(this);

            canvas.MouseDown += CanvasMouseDown;
            canvas.MouseMove += CanvasMouseMove;
            canvas.MouseUp += CanvasMouseUp;
            canvas.Paint += CanvasPaint;

            // ========== 初始化颜色按钮 ==========
            InitColorButtons();
        }
        private void TrackBarPenWidth_Scroll(object sender, EventArgs e)
        {
            _penWidth = trackBarPenWidth.Value;
            lblPenWidthValue.Text = $"粗细：{_penWidth}";

            // 可选：同步更新 NumericUpDown 的值（如果希望两者联动）
            // 找到之前创建的 NumericUpDown 控件，更新其 Value
            // 但注意 NumericUpDown 可能位于菜单中，需要先获取其引用
            // 这里演示如何通过遍历菜单项找到它（假设它被添加到了 cmsDrawTools3）
            foreach (ToolStripItem item in cmsDrawTools3.Items)
            {
                if (item is ToolStripControlHost host && host.Control is NumericUpDown nud)
                {
                    nud.Value = _penWidth;  // 注意：可能触发 ValueChanged 导致循环，需谨慎处理
                    break;
                }
            }
        }
        // ========== 初始化颜色按钮逻辑 ==========
        private void InitColorButtons()
        {
            // 1. 初始化button1背景色为默认画笔颜色（黑色）
            if (button1 != null)
            {
                button1.BackColor = _penColor;
                // 给button1加边框，方便区分颜色
                button1.FlatStyle = FlatStyle.Flat;
                button1.FlatAppearance.BorderColor = Color.Gray;
                button1.FlatAppearance.BorderSize = 2;
            }

            // 2. 绑定button2~button25的点击事件
            BindColorButtonClickEvents();
        }

        // ========== 绑定button2~button25点击事件 ==========
        private void BindColorButtonClickEvents()
        {
            // 获取窗体中所有名称为button2~button25的控件
            for (int i = 2; i <= 25; i++)
            {
                Control ctrl = this.Controls.Find($"button{i}", true).FirstOrDefault();
                if (ctrl is Button btn)
                {
                    btn.Click += (sender, e) =>
                    {
                        // 将点击的按钮背景色设置为画笔颜色
                        _penColor = btn.BackColor;
                        // 同步更新button1的背景色
                        if (button1 != null)
                        {
                            button1.BackColor = _penColor;
                        }
                        // 同步更新颜色菜单的背景色
                        if (toolStripMenuItem3 != null)
                        {
                            toolStripMenuItem3.BackColor = _penColor;
                        }
                    };

                    // 给颜色按钮加样式，提升视觉效果
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.LightGray;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Width = 30;
                    btn.Height = 30;
                }
            }
        }

        // ========== 重写：更新ChooseColor方法，同步button1颜色 ==========
        private void ChooseColor()
        {
            using (ColorDialog cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    _penColor = cd.Color;
                    toolStripMenuItem3.BackColor = _penColor;
                    // 同步更新button1背景色
                    if (button1 != null)
                    {
                        button1.BackColor = _penColor;
                    }
                }
            }
        }

        // [Modified] 加载历史图像，并同步到远程客户端（如果是服务端）
        public void LoadHistoryImage(Image image, int? historyId = null)
        {
            // 注意：传入的 image 是调用者创建的副本，我们需要自己释放
            using (image)
            {
                _graphics.Clear(Color.White);
                _graphics.DrawImage(image, 0, 0);
                canvas.Invalidate();
            }
            _currentHistoryId = historyId;
            CancelPreview();
            IsDrawing = false;

            // 如果是服务端，则广播历史图像给所有客户端（异步执行，避免阻塞UI）
            if (_isServerMode)
            {
                // 异步广播，使用当前画板图像 _bitmap 的副本进行发送
                BroadcastHistoryImageAsync(_bitmap);
            }
        }

        // [Modified] 异步广播历史图像（服务端专用）
        private void BroadcastHistoryImageAsync(Image image)
        {
            if (!_isServerMode || _drawServer == null) return;

            // 在后台线程中处理网络发送
            Task.Run(() =>
            {
                // 创建当前画板的副本，避免后续绘图修改影响发送数据
                Bitmap copy;
                lock (_bitmap)  // 简单同步，确保复制时不被其他线程修改（绘图主要在UI线程，但以防万一）
                {
                    copy = new Bitmap(_bitmap);
                }

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        copy.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] bytes = ms.ToArray();
                        string base64 = Convert.ToBase64String(bytes);
                        // 移除可能存在的换行符（虽然标准 Base64 无换行，但保险起见）
                        base64 = base64.Replace("\r", "").Replace("\n", "");
                        string cmd = $"LOAD_HISTORY_IMAGE|{base64}\n";
                        // 使用排除自我的广播方法，避免本机客户端再次触发加载
                        _drawServer.BroadcastToAllExceptSelf(cmd);
                    }
                }
                finally
                {
                    copy.Dispose();
                }
            });
        }

        private void NewDocument()
        {
            _graphics.Clear(Color.White);
            canvas.Invalidate();
            _currentHistoryId = null;   // 新建文档后不再关联任何历史记录
        }

        private void SaveDocument()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG图片|*.png|JPEG图片|*.jpg|Bitmap图片|*.bmp";
                sfd.DefaultExt = "png";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _bitmap.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);

                    string historyName = Path.GetFileNameWithoutExtension(sfd.FileName);
                    if (string.IsNullOrEmpty(historyName))
                        historyName = $"历史绘画 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                    Bitmap copy = new Bitmap(_bitmap);

                    if (_currentHistoryId.HasValue)
                    {
                        // 更新现有历史记录
                        Task.Run(() => UpdateHistoryInDatabase(_currentHistoryId.Value, copy));
                    }
                    else
                    {
                        // 插入新历史记录
                        Task.Run(() => SaveHistoryToDatabase(historyName, copy));
                    }

                    // 通知其他客户端也保存历史（保持原有逻辑）
                    SendSaveHistoryCommand();

                    MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void UpdateHistoryInDatabase(int historyId, Bitmap image)
        {
            try
            {
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }

                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                string connStr = $"Data Source={dbPath};Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string updateSql = "UPDATE PaintingHistory SET ImageData = @ImageData, CreateTime = datetime('now','localtime') WHERE Id = @Id";
                    using (SQLiteCommand cmd = new SQLiteCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", historyId);
                        cmd.Parameters.AddWithValue("@ImageData", imageBytes);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // 可记录日志，避免干扰用户
            }
            finally
            {
                image.Dispose();
            }
        }
        /// <summary>
        /// 将当前画板内容保存到历史数据库（不弹出文件保存对话框）
        /// </summary>
        private void SaveCurrentToHistory()
        {
            string name = $"远程同步保存 {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            Bitmap copy = new Bitmap(_bitmap);
            Task.Run(() => SaveHistoryToDatabase(name, copy));
        }

        /// <summary>
        /// 发送远程保存历史命令
        /// </summary>
        private void SendSaveHistoryCommand()
        {
            Task.Run(() =>
            {
                try
                {
                    _drawClient.Send("SAVE_HISTORY\n");
                }
                catch { }
            });
        }

        private void SaveHistoryToDatabase(string name, Bitmap image)//把用户画的图存进数据库，
        {
            try
            {
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }

                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                string connStr = $"Data Source={dbPath};Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string insertSql = "INSERT INTO PaintingHistory (Name, ImageData) VALUES (@Name, @ImageData);";
                    using (SQLiteCommand cmd = new SQLiteCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@ImageData", imageBytes);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // 可记录日志，避免干扰用户
            }
            finally
            {
                image.Dispose();
            }
        }

        private void CanvasMouseDown(object sender, MouseEventArgs e)
        {
            CancelPreview();
            _currentTool?.MouseDown(e);
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            _currentTool?.MouseMove(e);
        }

        private void CanvasMouseUp(object sender, MouseEventArgs e)
        {
            _currentTool?.MouseUp(e);
            CancelPreview();
        }

        private void CanvasPaint(object sender, PaintEventArgs e)//预览
        {
            if (_isPreviewing && _currentTool != null)
            {
                using (Pen pen = new Pen(_penColor, _penWidth))
                {
                    switch (_previewShape)
                    {
                        case PreviewShape.Line:
                            e.Graphics.DrawLine(pen, _previewStart, _previewEnd);
                            break;
                        case PreviewShape.Rectangle:
                            e.Graphics.DrawRectangle(pen, GetRectangle(_previewStart, _previewEnd));
                            break;
                        case PreviewShape.Ellipse:
                            e.Graphics.DrawEllipse(pen, GetRectangle(_previewStart, _previewEnd));
                            break;
                        case PreviewShape.Circle:
                            int side = Math.Max(Math.Abs(_previewStart.X - _previewEnd.X), Math.Abs(_previewStart.Y - _previewEnd.Y));
                            e.Graphics.DrawEllipse(pen, new Rectangle(
                                Math.Min(_previewStart.X, _previewEnd.X),
                                Math.Min(_previewStart.Y, _previewEnd.Y),
                                side, side));
                            break;
                    }
                }
            }
        }

        private Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y)
            );
        }

        public void CancelPreview()//退出预览
        {
            if (_isPreviewing)
            {
                _isPreviewing = false;
                canvas.Invalidate();
            }
        }

        public void StartPreview(Point start, PreviewShape shape)//开始预览
        {
            _isPreviewing = true;
            _previewShape = shape;
            _previewStart = start;
            _previewEnd = start;
            canvas.Invalidate();
        }

        public void UpdatePreview(Point current)//更新预览
        {
            if (_isPreviewing)
            {
                _previewEnd = current;
                canvas.Invalidate();
            }
        }

        private void InitSQLiteDb()
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                string connStr = $"Data Source={dbPath};Version=3;";

                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    string createDrawRecordsSql = @"
                        CREATE TABLE IF NOT EXISTS DrawRecords (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            RoomId TEXT,
                            ToolType TEXT,
                            StartX INTEGER,
                            StartY INTEGER,
                            EndX INTEGER,
                            EndY INTEGER,
                            Color INTEGER,
                            PenWidth INTEGER,
                            CreateTime DATETIME DEFAULT (datetime('now','localtime'))
                        );";

                    string createHistorySql = @"
                        CREATE TABLE IF NOT EXISTS PaintingHistory (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            CreateTime DATETIME DEFAULT (datetime('now','localtime')),
                            ImageData BLOB NOT NULL
                        );";

                    using (SQLiteCommand cmd = new SQLiteCommand(createDrawRecordsSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand(createHistorySql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"数据库初始化失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveDrawToDb(string toolType, Point start, Point end)//异步保存绘图记录到SQLite数据库
        {
            Task.Run(() =>
            {
                try
                {
                    string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                    string connStr = $"Data Source={dbPath};Version=3;";
                    using (SQLiteConnection conn = new SQLiteConnection(connStr))
                    {
                        conn.Open();
                        string insertSql = @"
                            INSERT INTO DrawRecords (RoomId, ToolType, StartX, StartY, EndX, EndY, Color, PenWidth)
                            VALUES (@RoomId, @ToolType, @StartX, @StartY, @EndX, @EndY, @Color, @PenWidth);";
                        using (SQLiteCommand cmd = new SQLiteCommand(insertSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@RoomId", _currentRoomId);
                            cmd.Parameters.AddWithValue("@ToolType", toolType);
                            cmd.Parameters.AddWithValue("@StartX", start.X);
                            cmd.Parameters.AddWithValue("@StartY", start.Y);
                            cmd.Parameters.AddWithValue("@EndX", end.X);
                            cmd.Parameters.AddWithValue("@EndY", end.Y);
                            cmd.Parameters.AddWithValue("@Color", _penColor.ToArgb());
                            cmd.Parameters.AddWithValue("@PenWidth", _penWidth);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { }
            });
        }

        // [Modified] 解析远程指令，增加详细错误捕获
        public void DrawFromRemote(string drawCmd)
        {
            try
            {
                // 处理远程保存历史命令
                if (drawCmd == "SAVE_HISTORY")
                {
                    SaveCurrentToHistory();
                    return;
                }

                // 处理历史图像加载命令
                if (drawCmd.StartsWith("LOAD_HISTORY_IMAGE|"))
                {
                    string base64 = drawCmd.Substring("LOAD_HISTORY_IMAGE|".Length);
                    try
                    {
                        byte[] bytes = Convert.FromBase64String(base64);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            // 从流中创建临时图像，并复制为独立 Bitmap
                            using (Image tempImg = Image.FromStream(ms))
                            {
                                Bitmap copy = new Bitmap(tempImg);
                                // 注意：LoadHistoryImage 内部会释放传入的图像
                                LoadHistoryImage(copy, null);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"加载历史图片失败：{ex.Message}\nBase64长度：{base64.Length}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                string[] parts = drawCmd.Split('|');
                if (parts.Length < 7) return;

                string toolType = parts[0];
                int startX = int.Parse(parts[1]);
                int startY = int.Parse(parts[2]);
                int endX = int.Parse(parts[3]);
                int endY = int.Parse(parts[4]);
                Color color = Color.FromArgb(int.Parse(parts[5]));
                int width = int.Parse(parts[6]);

                using (Pen pen = new Pen(color, width))
                {
                    switch (toolType)
                    {
                        case "Line":
                            _graphics.DrawLine(pen, new Point(startX, startY), new Point(endX, endY));
                            break;
                        case "Rect":
                            _graphics.DrawRectangle(pen, GetRectangle(new Point(startX, startY), new Point(endX, endY)));
                            break;
                        case "Ellipse":
                            _graphics.DrawEllipse(pen, GetRectangle(new Point(startX, startY), new Point(endX, endY)));
                            break;
                        case "Circle":
                            int side = Math.Max(Math.Abs(startX - endX), Math.Abs(startY - endY));
                            _graphics.DrawEllipse(pen, new Rectangle(
                                Math.Min(startX, endX),
                                Math.Min(startY, endY),
                                side, side));
                            break;
                        case "Eraser":
                            using (Pen erasePen = new Pen(Color.White, width * 2))
                            {
                                _graphics.DrawEllipse(erasePen, startX - width, startY - width, width * 2, width * 2);
                            }
                            break;
                        case "Curve":
                            FillLine(_graphics, new Point(startX, startY), new Point(endX, endY), color, width);
                            break;
                        case "Text":
                            if (parts.Length >= 8)
                            {
                                string textBase64 = parts[7];
                                string text = Encoding.UTF8.GetString(Convert.FromBase64String(textBase64));
                                using (Font font = new Font("Segoe UI", 12F))
                                using (Brush brush = new SolidBrush(color))
                                {
                                    _graphics.DrawString(text, font, brush, new Point(startX, startY));
                                }
                            }
                            break;
                    }
                }
                canvas.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解析远程指令失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FillLine(Graphics g, Point p0, Point p1, Color color, int penWidth)
        {
            int x0 = p0.X, y0 = p0.Y, x1 = p1.X, y1 = p1.Y;
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            int radius = penWidth / 2;
            int diameter = penWidth;

            using (SolidBrush brush = new SolidBrush(color))
            {
                while (true)
                {
                    g.FillEllipse(brush, x0 - radius, y0 - radius, diameter, diameter);
                    if (x0 == x1 && y0 == y1) break;
                    e2 = err;
                    if (e2 > -dx) { err -= dy; x0 += sx; }
                    if (e2 < dy) { err += dx; y0 += sy; }
                }
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (canvas == null || canvas.ClientSize.Width <= 0 || canvas.ClientSize.Height <= 0)
                return;

            Bitmap oldBitmap = _bitmap;
            try
            {
                var newBitmap = new Bitmap(canvas.Width, canvas.Height);
                var newGraphics = Graphics.FromImage(newBitmap);
                newGraphics.Clear(Color.White);
                if (oldBitmap != null)
                {
                    newGraphics.DrawImage(oldBitmap, 0, 0);
                }
                canvas.Image = newBitmap;
            }
            finally
            {
                oldBitmap?.Dispose();
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) { }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // ====== 新增：初始化 TrackBar 滑块 ======
            trackBarPenWidth.Minimum = 1;
            trackBarPenWidth.Maximum = 20;
            trackBarPenWidth.Value = _penWidth;       // 同步当前粗细
            trackBarPenWidth.TickFrequency = 5;
            trackBarPenWidth.SmallChange = 1;
            trackBarPenWidth.LargeChange = 5;
            trackBarPenWidth.Scroll += TrackBarPenWidth_Scroll; // 滑块滚动事件

            lblPenWidthValue.Text = $"粗细：{_penWidth}";
            lblPenWidthValue.AutoSize = true;

            // 画笔宽度 NumericUpDown
            NumericUpDown nudWidth = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 50,
                Value = _penWidth,
                Width = 80,
                Font = new Font("Segoe UI", 8.5f)
            };

            ToolStripControlHost host = new ToolStripControlHost(nudWidth)
            {
                Margin = new Padding(5, 2, 5, 2),
                AutoSize = false,
                Width = 100
            };

            cmsDrawTools3.Items.Add(host);

            nudWidth.ValueChanged += (s, args) => _penWidth = (int)nudWidth.Value;

            // IP 地址输入框
            TextBox txtIpInput = new TextBox
            {
                Font = new Font("Segoe UI", 8.5f),
                Width = 120,
                Text = "192.168.15.187"   // 设置默认IP
            };
            _menuIpTextBox = txtIpInput; // 保存引用

            ToolStripControlHost ipHost = new ToolStripControlHost(txtIpInput)
            {
                Margin = new Padding(5, 2, 5, 2),
                AutoSize = false,
                Width = 130
            };

            ToolStripLabel ipLabel = new ToolStripLabel("服务器IP：")
            {
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f)
            };

            cmsDrawTools4.Items.Add(ipLabel);
            cmsDrawTools4.Items.Add(ipHost);
        }

        // ========== 绘图工具抽象类及实现 ==========
        public abstract class DrawingTool
        {
            protected readonly Form1 Form;
            public DrawingTool(Form1 form) => Form = form;
            public abstract void MouseDown(MouseEventArgs e);
            public abstract void MouseMove(MouseEventArgs e);
            public abstract void MouseUp(MouseEventArgs e);
            protected void SendDrawCmd(string toolType, Point start, Point end)
            {
                Task.Run(() =>
                {
                    try
                    {
                        string cmd = $"{toolType}|{start.X}|{start.Y}|{end.X}|{end.Y}|{Form.PenColor.ToArgb()}|{Form.PenWidth}\n";
                        Form._drawClient.Send(cmd);
                    }
                    catch { }
                });
            }
        }

        public class LineTool : DrawingTool
        {
            public LineTool(Form1 form) : base(form) { }
            public override void MouseDown(MouseEventArgs e)
            {
                Form.StartPoint = e.Location;
                Form.StartPreview(e.Location, PreviewShape.Line);
                Form.IsDrawing = true;
            }
            public override void MouseMove(MouseEventArgs e)
            {
                if (Form.IsDrawing) Form.UpdatePreview(e.Location);
            }
            public override void MouseUp(MouseEventArgs e)
            {
                if (Form.IsDrawing)
                {
                    Form.CancelPreview();
                    Form.IsDrawing = false;
                    using (Pen pen = new Pen(Form.PenColor, Form.PenWidth))
                        Form.Graphics.DrawLine(pen, Form.StartPoint, e.Location);
                    Form.Canvas.Invalidate();
                    Form.SaveDrawToDb("Line", Form.StartPoint, e.Location);
                    SendDrawCmd("Line", Form.StartPoint, e.Location);
                }
            }
        }

        public class RectangleTool : DrawingTool
        {
            public RectangleTool(Form1 form) : base(form) { }
            public override void MouseDown(MouseEventArgs e)
            {
                Form.StartPoint = e.Location;
                Form.StartPreview(e.Location, PreviewShape.Rectangle);
                Form.IsDrawing = true;
            }
            public override void MouseMove(MouseEventArgs e)
            {
                if (Form.IsDrawing) Form.UpdatePreview(e.Location);
            }
            public override void MouseUp(MouseEventArgs e)
            {
                if (Form.IsDrawing)
                {
                    Form.CancelPreview();
                    Form.IsDrawing = false;
                    Rectangle rect = Form.GetRectangle(Form.StartPoint, e.Location);
                    using (Pen pen = new Pen(Form.PenColor, Form.PenWidth))
                        Form.Graphics.DrawRectangle(pen, rect);
                    Form.Canvas.Invalidate();
                    Form.SaveDrawToDb("Rect", Form.StartPoint, e.Location);
                    SendDrawCmd("Rect", Form.StartPoint, e.Location);
                }
            }
        }

        public class EllipseTool : DrawingTool
        {
            public EllipseTool(Form1 form) : base(form) { }
            public override void MouseDown(MouseEventArgs e)
            {
                Form.StartPoint = e.Location;
                Form.StartPreview(e.Location, PreviewShape.Ellipse);
                Form.IsDrawing = true;
            }
            public override void MouseMove(MouseEventArgs e)
            {
                if (Form.IsDrawing) Form.UpdatePreview(e.Location);
            }
            public override void MouseUp(MouseEventArgs e)
            {
                if (Form.IsDrawing)
                {
                    Form.CancelPreview();
                    Form.IsDrawing = false;
                    Rectangle rect = Form.GetRectangle(Form.StartPoint, e.Location);
                    using (Pen pen = new Pen(Form.PenColor, Form.PenWidth))
                        Form.Graphics.DrawEllipse(pen, rect);
                    Form.Canvas.Invalidate();
                    Form.SaveDrawToDb("Ellipse", Form.StartPoint, e.Location);
                    SendDrawCmd("Ellipse", Form.StartPoint, e.Location);
                }
            }
        }

        public class CircleTool : DrawingTool
        {
            public CircleTool(Form1 form) : base(form) { }
            public override void MouseDown(MouseEventArgs e)
            {
                Form.StartPoint = e.Location;
                Form.StartPreview(e.Location, PreviewShape.Circle);
                Form.IsDrawing = true;
            }
            public override void MouseMove(MouseEventArgs e)
            {
                if (Form.IsDrawing) Form.UpdatePreview(e.Location);
            }
            public override void MouseUp(MouseEventArgs e)
            {
                if (Form.IsDrawing)
                {
                    Form.CancelPreview();
                    Form.IsDrawing = false;
                    int side = Math.Max(Math.Abs(Form.StartPoint.X - e.Location.X), Math.Abs(Form.StartPoint.Y - e.Location.Y));
                    Rectangle rect = new Rectangle(
                        Math.Min(Form.StartPoint.X, e.Location.X),
                        Math.Min(Form.StartPoint.Y, e.Location.Y),
                        side, side);
                    using (Pen pen = new Pen(Form.PenColor, Form.PenWidth))
                        Form.Graphics.DrawEllipse(pen, rect);
                    Form.Canvas.Invalidate();
                    Form.SaveDrawToDb("Circle", Form.StartPoint, e.Location);
                    SendDrawCmd("Circle", Form.StartPoint, e.Location);
                }
            }
        }

        public class EraserTool : DrawingTool
        {
            public EraserTool(Form1 form) : base(form) { }
            public override void MouseDown(MouseEventArgs e)
            {
                Form.IsDrawing = true;
                EraseAt(e.Location);
            }
            public override void MouseMove(MouseEventArgs e)
            {
                if (Form.IsDrawing) EraseAt(e.Location);
            }
            public override void MouseUp(MouseEventArgs e) => Form.IsDrawing = false;
            private void EraseAt(Point p)
            {
                using (Pen pen = new Pen(Color.White, Form.PenWidth * 2))
                    Form.Graphics.DrawEllipse(pen, p.X - Form.PenWidth, p.Y - Form.PenWidth, Form.PenWidth * 2, Form.PenWidth * 2);
                Form.Canvas.Invalidate();
                Form.SaveDrawToDb("Eraser", p, p);
                SendDrawCmd("Eraser", p, p);
            }
        }

        // [Modified] 文字工具：发送指令时包含 Base64 编码的文字
        public class TextTool : DrawingTool
        {
            public TextTool(Form1 form) : base(form) { }
            public override void MouseDown(MouseEventArgs e)
            {
                using (TextInputDialog dialog = new TextInputDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog.InputText))
                    {
                        using (Font font = new Font("Segoe UI", 12F))
                        using (Brush brush = new SolidBrush(Form.PenColor))
                            Form.Graphics.DrawString(dialog.InputText, font, brush, e.Location);
                        Form.Canvas.Invalidate();
                        Form.SaveDrawToDb("Text", e.Location, new Point(0, 0));

                        // 发送远程指令（包含文字）
                        string textBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(dialog.InputText));
                        string cmd = $"Text|{e.X}|{e.Y}|0|0|{Form.PenColor.ToArgb()}|{Form.PenWidth}|{textBase64}\n";
                        Task.Run(() =>
                        {
                            try
                            {
                                Form._drawClient.Send(cmd);
                            }
                            catch { }
                        });
                    }
                }
            }
            public override void MouseMove(MouseEventArgs e) { }
            public override void MouseUp(MouseEventArgs e) { }
        }

        public class CurveTool : DrawingTool
        {
            private Point _lastPoint;
            public CurveTool(Form1 form) : base(form) { }

            public override void MouseDown(MouseEventArgs e)
            {
                Form.IsDrawing = true;
                _lastPoint = e.Location;
            }

            public override void MouseMove(MouseEventArgs e)
            {
                if (Form.IsDrawing)
                {
                    Form.FillLine(Form.Graphics, _lastPoint, e.Location, Form.PenColor, Form.PenWidth);
                    Form.Canvas.Invalidate();
                    Form.SaveDrawToDb("Curve", _lastPoint, e.Location);
                    SendDrawCmd("Curve", _lastPoint, e.Location);
                    _lastPoint = e.Location;
                }
            }

            public override void MouseUp(MouseEventArgs e) => Form.IsDrawing = false;
        }
        // ========== 菜单事件 ==========
        private void btnHistory_Click(object sender, EventArgs e)
        {
            // ========== 修改：使用新的类名 DrawHistoryForm ==========
            using (DrawHistoryForm historyForm = new DrawHistoryForm(this))
            {
                historyForm.ShowDialog();
            }
        }

        private void 任意曲线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentTool = new CurveTool(this);
        }

        private void btnDoC_Click(object sender, EventArgs e)
        {
            cmsDrawTools2.Show(btnDoC, new Point(btnDoC.Width, 0));
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void 直线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentTool = new LineTool(this);
        }

        private void 矩形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentTool = new RectangleTool(this);
        }

        private void 椭圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentTool = new EllipseTool(this);
        }

        private void 圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentTool = new CircleTool(this);
        }

        private void 文字ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentTool = new TextTool(this);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        private void btnGraphic_Click(object sender, EventArgs e)
        {
            cmsDrawTools.Show(btnGraphic, new Point(btnGraphic.Width, 0));
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ChooseColor();
        }

        private void btnPen_Click(object sender, EventArgs e)
        {
            cmsDrawTools3.Show(btnPen, new Point(btnPen.Width, 0));
        }

        private void btnEraser_Click(object sender, EventArgs e) { }

        private void 橡皮擦ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _currentTool = new EraserTool(this);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            try
            {
                _drawServer.Start(8888);
                _drawClient.Connect("127.0.0.1", 8888);

                // 获取本机客户端的 TcpClient 引用，并设置到 DrawServer 中以排除自身广播
                TcpClient localClient = _drawClient.Client;
                if (localClient != null)
                {
                    _drawServer.SetLocalClient(localClient);
                }

                _isServerMode = true;
                _lblStatus.Text = "服务端已启动（本机已连接）";
                _lblStatus.ForeColor = Color.LimeGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"启动服务端失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            try
            {
                string serverIp = _menuIpTextBox?.Text.Trim() ?? "";
                if (string.IsNullOrEmpty(serverIp))
                    serverIp = "192.168.15.187";

                _drawClient.Connect(serverIp, 8888);
                _lblStatus.Text = $"已连接到 {serverIp}";
                _lblStatus.ForeColor = Color.LimeGreen;
            }
            catch (Exception ex)
            {
                _lblStatus.Text = "连接失败";
                _lblStatus.ForeColor = Color.Red;
                MessageBox.Show($"连接服务器失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void butServer_Click(object sender, EventArgs e)
        {
            cmsDrawTools4.Show(butServer, new Point(butServer.Width, 0));
        }

        private void button26_Click(object sender, EventArgs e)
        {
            // 创建 MapForm 实例并显示
            MapForm mapForm = new MapForm();
            mapForm.Show(); // 非模态显示，两个窗体可同时操作
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }

    // ========== 文字输入对话框 ==========
    public class TextInputDialog : Form
    {
        private readonly TextBox _txtInput;
        public string InputText => _txtInput.Text;

        public TextInputDialog()
        {
            Text = "输入文字";
            Size = new Size(300, 120);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            _txtInput = new TextBox { Location = new Point(10, 10), Width = 260 };
            var btnOk = new Button { Text = "确定", DialogResult = DialogResult.OK, Location = new Point(114, 40) };
            var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(194, 40) };

            Controls.AddRange(new Control[] { _txtInput, btnOk, btnCancel });
        }
    }

    // ========== TCP协同通信类 ==========
    /// <summary>
    /// 服务器端通信类，负责监听客户端连接、接收消息、广播转发。
    /// </summary>
    public class DrawServer
    {
        private TcpListener _listener;               // TCP监听器，用于接受客户端连接
        private readonly List<TcpClient> _clients = new List<TcpClient>(); // 当前所有连接的客户端列表
        private readonly Form1 _form;                 // 主窗体引用（可能用于更新UI，但当前未使用）
        private Thread _listenThread;                  // 监听线程
        private TcpClient _localClient;                // 记录本机客户端（用于某些广播时排除自己）

        /// <summary>
        /// 构造函数，保存主窗体引用。
        /// </summary>
        /// <param name="form">主窗体对象</param>
        public DrawServer(Form1 form) => _form = form;

        /// <summary>
        /// 设置本机客户端引用，用于后续广播排除。
        /// </summary>
        /// <param name="client">本机的TcpClient对象</param>
        public void SetLocalClient(TcpClient client) => _localClient = client;

        /// <summary>
        /// 启动服务器，开始监听指定端口。
        /// </summary>
        /// <param name="port">监听端口号</param>
        public void Start(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port); // 监听所有网络接口
            _listener.Start();                                 // 启动监听
            _listenThread = new Thread(ListenForClients) { IsBackground = true }; // 创建后台线程
            _listenThread.Start();                             // 启动监听线程
        }

        /// <summary>
        /// 监听客户端连接的线程函数。
        /// 循环接受新客户端，加入列表，并为每个客户端启动处理线程。
        /// </summary>
        private void ListenForClients()
        {
            while (true)
            {
                try
                {
                    // 接受客户端连接（阻塞直到有客户端连接）
                    var client = _listener.AcceptTcpClient();
                    lock (_clients) _clients.Add(client);      // 线程安全地将新客户端加入列表
                                                               // 为每个客户端创建独立的后台线程处理通信
                    new Thread(HandleClientComm) { IsBackground = true }.Start(client);
                }
                catch
                {
                    // 如果监听器停止或发生异常，退出循环（静默处理）
                    break;
                }
            }
        }

        /// <summary>
        /// 处理单个客户端通信的线程函数。
        /// 从客户端读取数据，并广播给其他客户端。
        /// </summary>
        /// <param name="obj">传入的TcpClient对象（需转换）</param>
        private void HandleClientComm(object obj)
        {
            var client = (TcpClient)obj;               // 转换参数为TcpClient
            NetworkStream stream = null;                // 网络流对象
            try
            {
                stream = client.GetStream();            // 获取客户端的网络流
                                                        // 使用StreamReader按行读取（UTF-8编码，缓冲区1024，保留流打开）
                using (var reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true))
                {
                    string line;
                    // 循环读取每一行数据，直到客户端断开（ReadLine返回null）
                    while ((line = reader.ReadLine()) != null)
                    {
                        var cmd = line;                 // 获取接收到的命令字符串
                        Broadcast(cmd, client);         // 广播给其他所有客户端（排除发送者）
                                                        // 注释：服务端不再直接绘制，由客户端接收处理（即仅负责转发）
                    }
                }
            }
            catch
            {
                // 捕获所有异常（如连接重置、读取错误等），防止线程崩溃，静默处理
            }
            finally
            {
                // 确保客户端资源被释放并从列表中移除
                lock (_clients) _clients.Remove(client); // 从客户端列表中移除
                stream?.Close();                          // 关闭网络流（如果存在）
                client.Close();                           // 关闭TCP连接
            }
        }

        /// <summary>
        /// 将消息广播给所有已连接的客户端，排除指定的发送者。
        /// 注意：此方法在遍历时直接删除客户端，可能导致集合修改异常，需优化。
        /// </summary>
        /// <param name="message">要发送的消息内容</param>
        /// <param name="excludeClient">需要排除的客户端（通常为消息发送者）</param>
        private void Broadcast(string message, TcpClient excludeClient)
        {
            lock (_clients) // 线程安全地访问客户端列表
            {
                // 使用foreach遍历，如果在循环体内修改集合（如_Remove），会引发InvalidOperationException
                foreach (var client in _clients)
                {
                    if (client != excludeClient && client.Connected) // 不是发送者且连接正常
                    {
                        try
                        {
                            // 将消息转换为UTF-8字节数组，并追加换行符作为消息结束标记
                            var buffer = Encoding.UTF8.GetBytes(message + "\n");
                            client.GetStream().Write(buffer, 0, buffer.Length); // 发送数据
                        }
                        catch
                        {
                            // 发送失败，尝试从列表中移除客户端（但此处直接移除会破坏foreach枚举）
                            _clients.Remove(client); // 潜在问题：在foreach中修改集合将引发异常
                                                     // 建议改为收集待删除的客户端，循环结束后再移除，或使用for反向遍历
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 向除本机客户端外的所有已连接客户端广播消息。
        /// 此方法采用反向遍历，可安全地在遍历过程中移除失效客户端。
        /// </summary>
        /// <param name="message">要广播的消息内容（注意：不会自动追加换行符，调用方需确保格式）</param>
        public void BroadcastToAllExceptSelf(string message)
        {
            lock (_clients)
            {
                // 反向遍历列表，这样可以在遍历过程中安全地移除元素（删除不会影响前面未遍历的索引）
                for (int i = _clients.Count - 1; i >= 0; i--)
                {
                    var client = _clients[i];
                    if (client == _localClient) continue;  // 跳过本机客户端（不发送）

                    if (client.Connected)                  // 检查客户端是否仍连接
                    {
                        try
                        {
                            // 将消息转换为字节数组（不自动添加换行符，由调用者控制）
                            var buffer = Encoding.UTF8.GetBytes(message);
                            client.GetStream().Write(buffer, 0, buffer.Length); // 发送
                        }
                        catch
                        {
                            // 发送失败，从列表中移除该客户端并关闭连接
                            _clients.RemoveAt(i);
                            client.Close();
                        }
                    }
                    else
                    {
                        // 客户端已断开，从列表中移除
                        _clients.RemoveAt(i);
                    }
                }
            }
        }
    }

    public class DrawClient
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private readonly Form1 _form;
        private Thread _receiveThread;

        // 新增：公开 TcpClient 对象，供服务端设置本地客户端引用
        public TcpClient Client => _client;

        public DrawClient(Form1 form) => _form = form;

        public void Connect(string ip, int port)
        {
            if (_client != null && _client.Connected)
                _client.Close();
            _client = new TcpClient();
            _client.Connect(ip, port);
            _stream = _client.GetStream();
            _receiveThread = new Thread(ReceiveMessages) { IsBackground = true };
            _receiveThread.Start();
        }

        public void Send(string message)
        {
            if (_client != null && _client.Connected)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                _stream.Write(buffer, 0, buffer.Length);
            }
        }

        private void ReceiveMessages()
        {
            try
            {
                using (var reader = new StreamReader(_stream, Encoding.UTF8, false, 1024, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var cmd = line;
                        _form.Invoke(new Action(() => _form.DrawFromRemote(cmd)));
                    }
                }
            }
            catch { }
        }

    }

    // ========== 修改类名：HistoryForm → DrawHistoryForm，避免冲突 ==========
    // ========== 历史记录窗体 ==========
    public class DrawHistoryForm : Form
    {
        private readonly Form1 _mainForm;
        private ListView listViewHistory;
        private ImageList imageList;
        private Button btnLoad;
        private Button btnClose;

        public DrawHistoryForm(Form1 form)
        {
            _mainForm = form;
            InitializeComponent();
            this.Load += DrawHistoryForm_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "历史记录";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // 创建 ImageList（用于显示缩略图）
            imageList = new ImageList();
            imageList.ImageSize = new Size(100, 100);
            imageList.ColorDepth = ColorDepth.Depth32Bit;

            // 创建 ListView
            listViewHistory = new ListView();
            listViewHistory.Dock = DockStyle.Fill;
            listViewHistory.View = View.LargeIcon;
            listViewHistory.LargeImageList = imageList;
            listViewHistory.MultiSelect = false;
            listViewHistory.DoubleClick += ListViewHistory_DoubleClick;

            // 底部按钮面板
            Panel panelButtons = new Panel();
            panelButtons.Height = 50;
            panelButtons.Dock = DockStyle.Bottom;

            btnLoad = new Button();
            btnLoad.Text = "加载";
            btnLoad.Location = new Point(panelButtons.Width - 180, 10);
            btnLoad.Size = new Size(80, 30);
            btnLoad.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnLoad.Click += BtnLoad_Click;

            btnClose = new Button();
            btnClose.Text = "关闭";
            btnClose.Location = new Point(panelButtons.Width - 90, 10);
            btnClose.Size = new Size(80, 30);
            btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            btnClose.Click += (s, e) => this.Close();

            panelButtons.Controls.Add(btnLoad);
            panelButtons.Controls.Add(btnClose);

            this.Controls.Add(listViewHistory);
            this.Controls.Add(panelButtons);
        }

        private void DrawHistoryForm_Load(object sender, EventArgs e)
        {
            LoadHistoryRecords();
        }

        private void LoadHistoryRecords()
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                string connStr = $"Data Source={dbPath};Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT Id, Name, CreateTime, ImageData FROM PaintingHistory ORDER BY CreateTime DESC";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            // CreateTime 列可能为 NULL，但此处假设有值
                            byte[] imageData = (byte[])reader["ImageData"];

                            // 生成缩略图
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                using (Image original = Image.FromStream(ms))
                                {
                                    Image thumb = original.GetThumbnailImage(100, 100, null, IntPtr.Zero);
                                    imageList.Images.Add(id.ToString(), thumb);
                                }
                            }

                            ListViewItem item = new ListViewItem(name);
                            item.ImageKey = id.ToString();
                            item.Tag = id; // 存储记录ID，用于加载时查询原图
                            listViewHistory.Items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载历史记录失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadSelectedHistory();
        }

        private void ListViewHistory_DoubleClick(object sender, EventArgs e)
        {
            LoadSelectedHistory();
        }

        private void LoadSelectedHistory()
        {
            if (listViewHistory.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择一条历史记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)listViewHistory.SelectedItems[0].Tag;
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DrawHistory.db");
                string connStr = $"Data Source={dbPath};Version=3;";
                using (SQLiteConnection conn = new SQLiteConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT ImageData FROM PaintingHistory WHERE Id = @Id";
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        byte[] imageData = (byte[])cmd.ExecuteScalar();
                        if (imageData != null)
                        {
                            using (MemoryStream ms = new MemoryStream(imageData))
                            {
                                using (Image tempImg = Image.FromStream(ms))
                                {
                                    Bitmap copy = new Bitmap(tempImg);
                                    _mainForm.LoadHistoryImage(copy, id);  // 传递历史记录ID
                                }
                            }
                        }
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图片失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}