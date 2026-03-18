using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DrawBoard_self_
{
    public partial class MapForm : Form
    {
        public MapForm()
        {
            InitializeComponent();

            EnsureDataGridViewColumns();
        }
        private void EnsureDataGridViewColumns()
        {
            if (dgvPoints.Columns.Count == 0)
            {
                dgvPoints.Columns.Add("Longitude", "经度");
                dgvPoints.Columns.Add("Latitude", "纬度");
            }
        }
        private async void MapForm_Load(object sender, EventArgs e)
        {
            // 初始化 WebView2
            await webView21.EnsureCoreWebView2Async(null);

            // 订阅来自 HTML 的消息（显示经纬度）
            webView21.CoreWebView2.WebMessageReceived += (s, args) =>
            {
                var json = args.WebMessageAsJson;
                dynamic obj = JsonConvert.DeserializeObject(json);
                if (obj.type == "mapClick")
                {
                    double lng = obj.lng;
                    double lat = obj.lat;
                    lblCoordinates.Text = $"经度: {lng:F6}, 纬度: {lat:F6}";
                }
                else if (obj.type == "pointAdded")   // 实时更新表格
                {
                    var points = obj.points.ToObject<List<List<double>>>();
                    UpdatePointsTable(points);
                }
                // 可处理 drawFinished 等其他消息
            };

            // 导航到地图 HTML 文件
            string htmlPath = Path.Combine(Application.StartupPath, "Html", "GaodeMap.html");
            webView21.CoreWebView2.Navigate(htmlPath);
        }

        private void UpdatePointsTable(List<List<double>> points)
        {
            if (dgvPoints.InvokeRequired)
            {
                dgvPoints.Invoke(new Action(() => UpdatePointsTable(points)));
                return;
            }

            dgvPoints.Rows.Clear();
            foreach (var p in points)
            {
                dgvPoints.Rows.Add(p[0].ToString("F6"), p[1].ToString("F6"));
            }
        }

        private async void btnDrawPolyline_Click(object sender, EventArgs e)
        {
            if (webView21.CoreWebView2 == null) return;

            try
            {
                // 直接调用 HTML 中定义的函数，不再注入复杂脚本
                string result = await webView21.CoreWebView2.ExecuteScriptAsync("startDrawingPolyline()");
                if (!string.IsNullOrEmpty(result) && result != "null" && result != "\"null\"")
                {
                    // 可选显示返回值
                    // MessageBox.Show("JS 返回: " + result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("调用失败: " + ex.Message);
            }
        }

        private async void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (webView21.CoreWebView2 == null) return;

            try
            {
                // 调用 JS 函数获取当前路径
                string jsonResult = await webView21.CoreWebView2.ExecuteScriptAsync("getCurrentPath()");

                // 去除可能的外层引号（ExecuteScriptAsync 返回的字符串可能被额外包裹引号）
                jsonResult = jsonResult.Trim('"');

                // 如果缺少外层方括号，手动加上
                if (!jsonResult.StartsWith("[") && !jsonResult.EndsWith("]"))
                {
                    jsonResult = "[" + jsonResult + "]";
                }

                // 反序列化
                var points = JsonConvert.DeserializeObject<List<List<double>>>(jsonResult);
                if (points == null || points.Count == 0)
                {
                    MessageBox.Show("没有可导出的航线点，请先绘制一条航线。", "提示");
                    return;
                }

                // 保存为 CSV 文件
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV 文件|*.csv";
                sfd.DefaultExt = "csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var sw = new System.IO.StreamWriter(sfd.FileName, false, Encoding.UTF8))
                    {
                        sw.WriteLine("经度,纬度");
                        foreach (var p in points)
                        {
                            sw.WriteLine($"{p[0]:F6},{p[1]:F6}");
                        }
                    }
                    MessageBox.Show("航线导出成功！", "完成");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message);
            }
        }
    }
}
