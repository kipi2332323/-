namespace DrawBoard_self_
{
    partial class MapForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapForm));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDrawPolyline = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmsDrawTools = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.任意曲线ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.直线ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.矩形ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.椭圆ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.圆ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.文字ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.橡皮擦ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsDrawTools2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsDrawTools3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.sqLiteCommand1 = new System.Data.SQLite.SQLiteCommand();
            this.cmsDrawTools4 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.dgvPoints = new System.Windows.Forms.DataGridView();
            this.colLongitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLatitude = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblCoordinates = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmsDrawTools.SuspendLayout();
            this.cmsDrawTools2.SuspendLayout();
            this.cmsDrawTools3.SuspendLayout();
            this.cmsDrawTools4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "椭圆.png");
            this.imageList1.Images.SetKeyName(1, "矩形.png");
            this.imageList1.Images.SetKeyName(2, "圆.png");
            this.imageList1.Images.SetKeyName(3, "直线.png");
            this.imageList1.Images.SetKeyName(4, "保存.png");
            this.imageList1.Images.SetKeyName(5, "新建.png");
            this.imageList1.Images.SetKeyName(6, "颜色.png");
            this.imageList1.Images.SetKeyName(7, "文字.png");
            this.imageList1.Images.SetKeyName(8, "橡皮,擦除,橡皮擦.png");
            this.imageList1.Images.SetKeyName(9, "曲线.png");
            this.imageList1.Images.SetKeyName(10, "shape.png");
            this.imageList1.Images.SetKeyName(11, "画笔.png");
            this.imageList1.Images.SetKeyName(12, "文件.png");
            this.imageList1.Images.SetKeyName(13, "历史记录.png");
            this.imageList1.Images.SetKeyName(14, "网络.png");
            this.imageList1.Images.SetKeyName(15, "多人联机.png");
            this.imageList1.Images.SetKeyName(16, "网络 (1).png");
            this.imageList1.Images.SetKeyName(17, "网络 (2).png");
            this.imageList1.Images.SetKeyName(18, "网络 (3).png");
            this.imageList1.Images.SetKeyName(19, "网络 (4).png");
            this.imageList1.Images.SetKeyName(20, "internet (1).png");
            this.imageList1.Images.SetKeyName(21, "24gl-network.png");
            this.imageList1.Images.SetKeyName(22, "internet (1).png");
            this.imageList1.Images.SetKeyName(23, "internet.png");
            this.imageList1.Images.SetKeyName(24, "网络 (4).png");
            this.imageList1.Images.SetKeyName(25, "网络 (3).png");
            this.imageList1.Images.SetKeyName(26, "网络 (2).png");
            this.imageList1.Images.SetKeyName(27, "网络 (1).png");
            this.imageList1.Images.SetKeyName(28, "多人联机.png");
            this.imageList1.Images.SetKeyName(29, "连接 (1).png");
            this.imageList1.Images.SetKeyName(30, "线条粗细.png");
            this.imageList1.Images.SetKeyName(31, "颜色库.png");
            this.imageList1.Images.SetKeyName(32, "连接.png");
            this.imageList1.Images.SetKeyName(33, "启动.png");
            this.imageList1.Images.SetKeyName(34, "历史记录.png");
            this.imageList1.Images.SetKeyName(35, "网络.png");
            this.imageList1.Images.SetKeyName(36, "橡皮擦.png");
            this.imageList1.Images.SetKeyName(37, "橡皮擦工具.png");
            this.imageList1.Images.SetKeyName(38, "新建添加.png");
            this.imageList1.Images.SetKeyName(39, "保存 (2).png");
            this.imageList1.Images.SetKeyName(40, "保存 (1).png");
            this.imageList1.Images.SetKeyName(41, "新建 (1).png");
            this.imageList1.Images.SetKeyName(42, "shape.png");
            this.imageList1.Images.SetKeyName(43, "画笔.png");
            this.imageList1.Images.SetKeyName(44, "文件.png");
            this.imageList1.Images.SetKeyName(45, "文字颜色.png");
            this.imageList1.Images.SetKeyName(46, "椭圆 (1).png");
            this.imageList1.Images.SetKeyName(47, "圆型.png");
            this.imageList1.Images.SetKeyName(48, "对角画矩形.png");
            this.imageList1.Images.SetKeyName(49, "曲线修复.png");
            this.imageList1.Images.SetKeyName(50, "直线 (1).png");
            this.imageList1.Images.SetKeyName(51, "连接 (2).png");
            this.imageList1.Images.SetKeyName(52, "航线 (1).png");
            this.imageList1.Images.SetKeyName(53, "Excel.png");
            // 
            // btnDrawPolyline
            // 
            this.btnDrawPolyline.AutoSize = true;
            this.btnDrawPolyline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDrawPolyline.FlatAppearance.BorderSize = 0;
            this.btnDrawPolyline.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnDrawPolyline.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnDrawPolyline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDrawPolyline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDrawPolyline.ForeColor = System.Drawing.Color.White;
            this.btnDrawPolyline.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDrawPolyline.ImageIndex = 52;
            this.btnDrawPolyline.ImageList = this.imageList1;
            this.btnDrawPolyline.Location = new System.Drawing.Point(5, 5);
            this.btnDrawPolyline.Margin = new System.Windows.Forms.Padding(0);
            this.btnDrawPolyline.Name = "btnDrawPolyline";
            this.btnDrawPolyline.Padding = new System.Windows.Forms.Padding(50, 0, 20, 0);
            this.btnDrawPolyline.Size = new System.Drawing.Size(249, 50);
            this.btnDrawPolyline.TabIndex = 20;
            this.btnDrawPolyline.Text = "绘制航线";
            this.toolTip1.SetToolTip(this.btnDrawPolyline, "绘制航线");
            this.btnDrawPolyline.UseVisualStyleBackColor = true;
            this.btnDrawPolyline.Click += new System.EventHandler(this.btnDrawPolyline_Click);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.AutoSize = true;
            this.btnExportExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportExcel.FlatAppearance.BorderSize = 0;
            this.btnExportExcel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnExportExcel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnExportExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportExcel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnExportExcel.ForeColor = System.Drawing.Color.White;
            this.btnExportExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportExcel.ImageIndex = 53;
            this.btnExportExcel.ImageList = this.imageList1;
            this.btnExportExcel.Location = new System.Drawing.Point(5, 55);
            this.btnExportExcel.Margin = new System.Windows.Forms.Padding(0);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Padding = new System.Windows.Forms.Padding(50, 0, 20, 0);
            this.btnExportExcel.Size = new System.Drawing.Size(249, 50);
            this.btnExportExcel.TabIndex = 21;
            this.btnExportExcel.Text = "导出 Excel";
            this.toolTip1.SetToolTip(this.btnExportExcel, "导出excel表格");
            this.btnExportExcel.UseVisualStyleBackColor = true;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("华文行楷", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(908, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(388, 42);
            this.label2.TabIndex = 1;
            this.label2.Text = "日新自强     知行合一";
            // 
            // cmsDrawTools
            // 
            this.cmsDrawTools.BackColor = System.Drawing.Color.Black;
            this.cmsDrawTools.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsDrawTools.Font = new System.Drawing.Font("Microsoft YaHei UI", 5.4F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsDrawTools.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsDrawTools.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmsDrawTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.任意曲线ToolStripMenuItem,
            this.直线ToolStripMenuItem,
            this.矩形ToolStripMenuItem,
            this.椭圆ToolStripMenuItem,
            this.圆ToolStripMenuItem,
            this.文字ToolStripMenuItem,
            this.橡皮擦ToolStripMenuItem});
            this.cmsDrawTools.Name = "contextMenuStrip1";
            this.cmsDrawTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsDrawTools.Size = new System.Drawing.Size(145, 186);
            this.cmsDrawTools.Text = "图像";
            // 
            // 任意曲线ToolStripMenuItem
            // 
            this.任意曲线ToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.任意曲线ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.任意曲线ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.任意曲线ToolStripMenuItem.Image = global::DrawBoard_self_.Properties.Resources.曲线修复;
            this.任意曲线ToolStripMenuItem.Name = "任意曲线ToolStripMenuItem";
            this.任意曲线ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.任意曲线ToolStripMenuItem.Text = "任意曲线";
            // 
            // 直线ToolStripMenuItem
            // 
            this.直线ToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.直线ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.直线ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.直线ToolStripMenuItem.Image = global::DrawBoard_self_.Properties.Resources.直线__1_;
            this.直线ToolStripMenuItem.Name = "直线ToolStripMenuItem";
            this.直线ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.直线ToolStripMenuItem.Text = "直线";
            // 
            // 矩形ToolStripMenuItem
            // 
            this.矩形ToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.矩形ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.矩形ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.矩形ToolStripMenuItem.Image = global::DrawBoard_self_.Properties.Resources.对角画矩形;
            this.矩形ToolStripMenuItem.Name = "矩形ToolStripMenuItem";
            this.矩形ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.矩形ToolStripMenuItem.Text = "矩形";
            // 
            // 椭圆ToolStripMenuItem
            // 
            this.椭圆ToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.椭圆ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.椭圆ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.椭圆ToolStripMenuItem.Image = global::DrawBoard_self_.Properties.Resources.椭圆__1_;
            this.椭圆ToolStripMenuItem.Name = "椭圆ToolStripMenuItem";
            this.椭圆ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.椭圆ToolStripMenuItem.Text = "椭圆";
            // 
            // 圆ToolStripMenuItem
            // 
            this.圆ToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.圆ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.圆ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.圆ToolStripMenuItem.Image = global::DrawBoard_self_.Properties.Resources.圆型;
            this.圆ToolStripMenuItem.Name = "圆ToolStripMenuItem";
            this.圆ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.圆ToolStripMenuItem.Text = "圆";
            // 
            // 文字ToolStripMenuItem
            // 
            this.文字ToolStripMenuItem.BackColor = System.Drawing.Color.Black;
            this.文字ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.文字ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.文字ToolStripMenuItem.Image = global::DrawBoard_self_.Properties.Resources.文字颜色;
            this.文字ToolStripMenuItem.Name = "文字ToolStripMenuItem";
            this.文字ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.文字ToolStripMenuItem.Text = "文字";
            // 
            // 橡皮擦ToolStripMenuItem
            // 
            this.橡皮擦ToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.橡皮擦ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.橡皮擦ToolStripMenuItem.Image = global::DrawBoard_self_.Properties.Resources.橡皮擦;
            this.橡皮擦ToolStripMenuItem.Name = "橡皮擦ToolStripMenuItem";
            this.橡皮擦ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.橡皮擦ToolStripMenuItem.Text = "橡皮擦";
            // 
            // cmsDrawTools2
            // 
            this.cmsDrawTools2.BackColor = System.Drawing.Color.Black;
            this.cmsDrawTools2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsDrawTools2.Font = new System.Drawing.Font("Microsoft YaHei UI", 5.4F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsDrawTools2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsDrawTools2.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmsDrawTools2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.cmsDrawTools2.Name = "contextMenuStrip1";
            this.cmsDrawTools2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsDrawTools2.Size = new System.Drawing.Size(113, 56);
            this.cmsDrawTools2.Text = "图像";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.Color.Black;
            this.toolStripMenuItem1.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem1.Image = global::DrawBoard_self_.Properties.Resources.新建添加;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(112, 26);
            this.toolStripMenuItem1.Text = "新建";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.BackColor = System.Drawing.Color.Black;
            this.toolStripMenuItem2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.toolStripMenuItem2.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem2.Image = global::DrawBoard_self_.Properties.Resources.保存__2_;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(112, 26);
            this.toolStripMenuItem2.Text = "保存";
            // 
            // cmsDrawTools3
            // 
            this.cmsDrawTools3.BackColor = System.Drawing.Color.Black;
            this.cmsDrawTools3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsDrawTools3.Font = new System.Drawing.Font("Microsoft YaHei UI", 5.4F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsDrawTools3.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsDrawTools3.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmsDrawTools3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.cmsDrawTools3.Name = "contextMenuStrip1";
            this.cmsDrawTools3.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsDrawTools3.Size = new System.Drawing.Size(113, 56);
            this.cmsDrawTools3.Text = "图像";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.BackColor = System.Drawing.Color.Black;
            this.toolStripMenuItem3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.toolStripMenuItem3.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem3.Image = global::DrawBoard_self_.Properties.Resources.颜色库;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(112, 26);
            this.toolStripMenuItem3.Text = "颜色";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.BackColor = System.Drawing.Color.Black;
            this.toolStripMenuItem4.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.toolStripMenuItem4.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem4.Image = global::DrawBoard_self_.Properties.Resources.线条粗细;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(112, 26);
            this.toolStripMenuItem4.Text = "粗细";
            // 
            // sqLiteCommand1
            // 
            this.sqLiteCommand1.CommandText = null;
            // 
            // cmsDrawTools4
            // 
            this.cmsDrawTools4.BackColor = System.Drawing.Color.Black;
            this.cmsDrawTools4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsDrawTools4.Font = new System.Drawing.Font("Microsoft YaHei UI", 5.4F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsDrawTools4.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsDrawTools4.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmsDrawTools4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
            this.cmsDrawTools4.Name = "contextMenuStrip1";
            this.cmsDrawTools4.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.cmsDrawTools4.Size = new System.Drawing.Size(161, 56);
            this.cmsDrawTools4.Text = "图像";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.BackColor = System.Drawing.Color.Black;
            this.toolStripMenuItem5.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.toolStripMenuItem5.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem5.Image = global::DrawBoard_self_.Properties.Resources.启动;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(160, 26);
            this.toolStripMenuItem5.Text = "启动服务器";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.BackColor = System.Drawing.Color.Black;
            this.toolStripMenuItem6.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.toolStripMenuItem6.ForeColor = System.Drawing.Color.White;
            this.toolStripMenuItem6.Image = global::DrawBoard_self_.Properties.Resources.连接__1_;
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(160, 26);
            this.toolStripMenuItem6.Text = "连接服务器";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DrawBoard_self_.Properties.Resources.南昌航空;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(360, 77);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Black;
            this.flowLayoutPanel1.Controls.Add(this.btnDrawPolyline);
            this.flowLayoutPanel1.Controls.Add(this.btnExportExcel);
            this.flowLayoutPanel1.Controls.Add(this.dgvPoints);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 85);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(233, 760);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // dgvPoints
            // 
            this.dgvPoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPoints.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLongitude,
            this.colLatitude});
            this.dgvPoints.Location = new System.Drawing.Point(8, 125);
            this.dgvPoints.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.dgvPoints.Name = "dgvPoints";
            this.dgvPoints.RowHeadersVisible = false;
            this.dgvPoints.RowHeadersWidth = 51;
            this.dgvPoints.RowTemplate.Height = 27;
            this.dgvPoints.Size = new System.Drawing.Size(219, 497);
            this.dgvPoints.TabIndex = 3;
            // 
            // colLongitude
            // 
            this.colLongitude.FillWeight = 198F;
            this.colLongitude.HeaderText = "经度";
            this.colLongitude.MinimumWidth = 6;
            this.colLongitude.Name = "colLongitude";
            this.colLongitude.Width = 125;
            // 
            // colLatitude
            // 
            this.colLatitude.FillWeight = 98F;
            this.colLatitude.HeaderText = "纬度";
            this.colLatitude.MinimumWidth = 6;
            this.colLatitude.Name = "colLatitude";
            this.colLatitude.Width = 125;
            // 
            // lblCoordinates
            // 
            this.lblCoordinates.AutoSize = true;
            this.lblCoordinates.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCoordinates.ForeColor = System.Drawing.Color.White;
            this.lblCoordinates.Location = new System.Drawing.Point(389, 60);
            this.lblCoordinates.Name = "lblCoordinates";
            this.lblCoordinates.Size = new System.Drawing.Size(57, 20);
            this.lblCoordinates.TabIndex = 19;
            this.lblCoordinates.Text = "坐标：";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel1.Controls.Add(this.lblCoordinates);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1540, 85);
            this.panel1.TabIndex = 5;
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(3, -48);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(1307, 760);
            this.webView21.TabIndex = 2;
            this.webView21.ZoomFactor = 1D;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.webView21);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(233, 85);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1307, 760);
            this.panel2.TabIndex = 6;
            // 
            // MapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1540, 845);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "MapForm";
            this.Text = "MapForm";
            this.Load += new System.EventHandler(this.MapForm_Load);
            this.cmsDrawTools.ResumeLayout(false);
            this.cmsDrawTools2.ResumeLayout(false);
            this.cmsDrawTools3.ResumeLayout(false);
            this.cmsDrawTools4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPoints)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnDrawPolyline;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip cmsDrawTools;
        private System.Windows.Forms.ToolStripMenuItem 任意曲线ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 直线ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 矩形ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 椭圆ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 圆ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 文字ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 橡皮擦ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsDrawTools2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ContextMenuStrip cmsDrawTools3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Data.SQLite.SQLiteCommand sqLiteCommand1;
        private System.Windows.Forms.ContextMenuStrip cmsDrawTools4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblCoordinates;
        private System.Windows.Forms.Panel panel1;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvPoints;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLongitude;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLatitude;
    }
}