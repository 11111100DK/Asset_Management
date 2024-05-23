using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace AsserManagement
{

    public partial class GUIStactic : UserControl
    {
        private DatabaseManager dbManager;
        private ToolTip currentToolTip; // Biến để lưu trữ tooltip hiện tại
        int selectedYear = DateTime.Now.Year;
        int selectedMonth = DateTime.Now.Month;

        public GUIStactic()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
            LoadRecentYears();
            LoadMonthsIntoComboBox();
            DrawLineChart();
        }
        private void DrawLineChart()
        {
            label2.Text = "Asset Management - Year " + selectedYear + " Statistical Chart";
            // Mở kết nối đến cơ sở dữ liệu
            if (dbManager.OpenConnection())
            {
                // Xóa tất cả các series trong biểu đồ
                chart1.Series.Clear();


                // Sử dụng điều kiện WHERE để lọc dữ liệu theo năm đã chọn
                string query = @"SELECT AllMonths.Month, COALESCE(Data.Total, 0) AS Total
                FROM
                (
                    SELECT 1 AS Month
                    UNION ALL SELECT 2
                    UNION ALL SELECT 3
                    UNION ALL SELECT 4
                    UNION ALL SELECT 5
                    UNION ALL SELECT 6
                    UNION ALL SELECT 7
                    UNION ALL SELECT 8
                    UNION ALL SELECT 9
                    UNION ALL SELECT 10
                    UNION ALL SELECT 11
                    UNION ALL SELECT 12
                ) AS AllMonths
                LEFT JOIN
                (
                    SELECT MONTH(PurchaseDate) AS Month, COUNT(*) AS Total 
                    FROM fixedassets 
                    WHERE YEAR(PurchaseDate) = @SelectedYear AND PurchaseDate IS NOT NULL
                    GROUP BY MONTH(PurchaseDate)
                ) AS Data
                ON AllMonths.Month = Data.Month;";

                // Thay thế @SelectedYear bằng giá trị năm đã chọn
                query = query.Replace("@SelectedYear", selectedYear.ToString());
                MySqlDataReader reader = dbManager.ExecuteQuery(query);

                // Tạo một đối tượng Series cho LineChart
                Series serieasset = new Series();
                serieasset.ChartType = SeriesChartType.Line;
                serieasset.BorderWidth = 3;
                serieasset.MarkerStyle = MarkerStyle.Circle; // Đặt kiểu chấm thành hình tròn
                serieasset.MarkerSize = 10; // Đặt kích thước chấm// Đặt độ dày của đường thành 3

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series
                while (reader.Read())
                {
                    int month = reader.GetInt32("Month");
                    int total = reader.GetInt32("Total");
                    serieasset.Points.AddXY(month, total);
                }

                // Đóng đối tượng MySqlDataReader và đóng kết nối
                reader.Close();


                // Truy vấn dữ liệu từ cơ sở dữ liệu cho RepairsAndMaintenance
                string queryRepairs = @"
    SELECT AllMonths.Month, COALESCE(Data.Total, 0) AS Total
    FROM
    (
        SELECT 1 AS Month UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6
        UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
    ) AS AllMonths
    LEFT JOIN
    (
        SELECT MONTH(RepairDate) AS Month, COUNT(*) AS Total 
        FROM repairsandmaintenance 
        WHERE RepairDate IS NOT NULL
        AND YEAR(RepairDate) = @SelectedYear
        GROUP BY MONTH(RepairDate)
    ) AS Data
    ON AllMonths.Month = Data.Month;
";
                queryRepairs = queryRepairs.Replace("@SelectedYear", selectedYear.ToString());

                MySqlDataReader readerRepairs = dbManager.ExecuteQuery(queryRepairs);

                // Tạo một đối tượng Series cho RepairsAndMaintenance
                Series seriesRepairs = new Series();
                seriesRepairs.ChartType = SeriesChartType.Line;
                seriesRepairs.BorderWidth = 3;
                seriesRepairs.MarkerStyle = MarkerStyle.Circle;
                seriesRepairs.MarkerSize = 10;
                seriesRepairs.Name = "Repairs";

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series cho RepairsAndMaintenance
                while (readerRepairs.Read())
                {
                    int monthRepairs = readerRepairs.GetInt32("Month");
                    int totalRepairs = readerRepairs.GetInt32("Total");
                    seriesRepairs.Points.AddXY(monthRepairs, totalRepairs);
                }

                // Đóng đối tượng MySqlDataReader cho RepairsAndMaintenance
                readerRepairs.Close();
                // Truy vấn dữ liệu từ cơ sở dữ liệu cho TransferDate
                string queryTransfers = @"
    SELECT AllMonths.Month, COALESCE(Data.Total, 0) AS Total
    FROM
    (
        SELECT 1 AS Month UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL
        SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL
        SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL
        SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
    ) AS AllMonths
    LEFT JOIN
    (
        SELECT MONTH(TransferDate) AS Month, COUNT(*) AS Total 
        FROM historytransferasset 
        WHERE TransferDate IS NOT NULL
            AND YEAR(TransferDate) = @SelectedYear
        GROUP BY MONTH(TransferDate)
    ) AS Data
    ON AllMonths.Month = Data.Month;
";
                queryTransfers = queryTransfers.Replace("@SelectedYear", selectedYear.ToString());

                MySqlDataReader readerTransfers = dbManager.ExecuteQuery(queryTransfers);

                // Tạo một đối tượng Series cho TransferDate
                Series seriesTransfers = new Series();
                seriesTransfers.ChartType = SeriesChartType.Line;
                seriesTransfers.BorderWidth = 3;
                seriesTransfers.MarkerStyle = MarkerStyle.Circle;
                seriesTransfers.MarkerSize = 10;
                seriesTransfers.Name = "Transfers";

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series cho TransferDate
                while (readerTransfers.Read())
                {
                    int monthTransfers = readerTransfers.GetInt32("Month");
                    int totalTransfers = readerTransfers.GetInt32("Total");
                    seriesTransfers.Points.AddXY(monthTransfers, totalTransfers);
                }

                // Đóng đối tượng MySqlDataReader cho TransferDate
                readerTransfers.Close();
                // Truy vấn dữ liệu từ cơ sở dữ liệu cho DisposalDate
                string queryDisposal = @"
    SELECT AllMonths.Month, COALESCE(Data.Total, 0) AS Total
    FROM
    (
        SELECT 1 AS Month UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL
        SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL
        SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9 UNION ALL
        SELECT 10 UNION ALL SELECT 11 UNION ALL SELECT 12
    ) AS AllMonths
    LEFT JOIN
    (
        SELECT MONTH(DisposalDate) AS Month, COUNT(*) AS Total 
        FROM disposal 
        WHERE DisposalDate IS NOT NULL
            AND YEAR(DisposalDate) = @SelectedYear
        GROUP BY MONTH(DisposalDate)
    ) AS Data
    ON AllMonths.Month = Data.Month;
";
                queryDisposal = queryDisposal.Replace("@SelectedYear", selectedYear.ToString());

                MySqlDataReader readerDisposal = dbManager.ExecuteQuery(queryDisposal);

                // Tạo một đối tượng Series cho DisposalDate
                Series seriesDisposal = new Series();
                seriesDisposal.ChartType = SeriesChartType.Line;
                seriesDisposal.BorderWidth = 3;
                seriesDisposal.MarkerStyle = MarkerStyle.Circle;
                seriesDisposal.MarkerSize = 10;
                seriesDisposal.Color = Color.Purple; // Chọn màu cho đường line
                seriesDisposal.Name = "Disposals";

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series cho DisposalDate
                while (readerDisposal.Read())
                {
                    int monthDisposal = readerDisposal.GetInt32("Month");
                    int totalDisposal = readerDisposal.GetInt32("Total");
                    seriesDisposal.Points.AddXY(monthDisposal, totalDisposal);
                }
                // Đóng đối tượng MySqlDataReader cho DisposalDate
                readerDisposal.Close();
                dbManager.CloseConnection();
                // Thêm Series vào chart1
                chart1.Series.Add(serieasset);
                // Thêm Series mới vào biểu đồ
                chart1.Series.Add(seriesRepairs);
                // Thêm Series mới vào biểu đồ
                chart1.Series.Add(seriesTransfers);
                // Loại bỏ cột đầu và cột cuối
                // Thêm Series mới vào biểu đồ
                chart1.Series.Add(seriesDisposal);
                // Định nghĩa các màu pastel
                Color pastelOrange = Color.FromArgb(255, 255, 192, 128); // Màu cam
                Color pastelBlue = Color.FromArgb(255, 128, 192, 255);   // Màu lam
                Color pastelGreen = Color.FromArgb(255, 128, 255, 128);  // Màu lục
                Color pastelPurple = Color.FromArgb(255, 192, 128, 255); // Màu tím

                // Đặt màu cho các series trong biểu đồ


                chart1.Series[0].Name = "Purchases";
                chart1.Series["Purchases"].Color = pastelOrange;
                chart1.Series["Repairs"].Color = pastelBlue;
                chart1.Series["Transfers"].Color = pastelGreen;
                chart1.Series["Disposals"].Color = pastelPurple;
                chart1.ChartAreas[0].AxisY.Minimum = 0; // Giá trị tối thiểu bạn muốn cố định
                chart1.ChartAreas[0].AxisY.Maximum = 50;
                chart1.ChartAreas[0].AxisX.Minimum = 1;
                chart1.ChartAreas[0].AxisX.Maximum = 12;
                chart1.ChartAreas[0].AxisX.Title = "Month";
                chart1.ChartAreas[0].AxisY.Title = "Asset";
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                // Đặt sự kiện MouseMove cho từng series
                chart1.MouseMove += Chart2_MouseMove;


            }
            else
            {
                // Xử lý khi không thể mở kết nối
                MessageBox.Show("Could not open connection to database.");
            }
        }
        private void DrawLineChart2()
        {
            label2.Text = "Asset Management - Year " + selectedYear + " - Month " + selectedMonth + " Statistical Chart";

            // Mở kết nối đến cơ sở dữ liệu
            if (dbManager.OpenConnection())
            {
                // Xóa tất cả các series trong biểu đồ
                chart1.Series.Clear();
                string query = @"
    SELECT Alldays.Day, COALESCE(Data.Total, 0) AS Total
    FROM
    (
        SELECT 1 AS Day
        UNION ALL SELECT 2
        UNION ALL SELECT 3
        UNION ALL SELECT 4
        UNION ALL SELECT 5
        UNION ALL SELECT 6
        UNION ALL SELECT 7
        UNION ALL SELECT 8
        UNION ALL SELECT 9
        UNION ALL SELECT 10
        UNION ALL SELECT 11
        UNION ALL SELECT 12
        UNION ALL SELECT 13
        UNION ALL SELECT 14
        UNION ALL SELECT 15
        UNION ALL SELECT 16
        UNION ALL SELECT 17
        UNION ALL SELECT 18
        UNION ALL SELECT 19
        UNION ALL SELECT 20
        UNION ALL SELECT 21
        UNION ALL SELECT 22  
        UNION ALL SELECT 23
        UNION ALL SELECT 24
        UNION ALL SELECT 25
        UNION ALL SELECT 26
        UNION ALL SELECT 27
        UNION ALL SELECT 28
        UNION ALL SELECT 29
        UNION ALL SELECT 30
        UNION ALL SELECT 31  ) AS AllDays
    LEFT JOIN
    (
        SELECT DAY(PurchaseDate) AS Day, COUNT(*) AS Total 
        FROM fixedassets 
        WHERE YEAR(PurchaseDate) = @SelectedYear AND MONTH(PurchaseDate) = @SelectedMonth AND PurchaseDate IS NOT NULL
        GROUP BY DAY(PurchaseDate)
    ) AS Data
    ON AllDays.Day = Data.Day;
";

                // Thay thế @SelectedYear và @SelectedMonth bằng giá trị năm và tháng đã chọn
                query = query.Replace("@SelectedYear", selectedYear.ToString()).Replace("@SelectedMonth", selectedMonth.ToString());

                MySqlDataReader reader = dbManager.ExecuteQuery(query);

                // Tạo một đối tượng Series cho LineChart
                Series serieasset = new Series();
                serieasset.ChartType = SeriesChartType.Line;
                serieasset.BorderWidth = 3;
                serieasset.MarkerStyle = MarkerStyle.Circle; // Đặt kiểu chấm thành hình tròn
                serieasset.MarkerSize = 10; // Đặt kích thước chấm// Đặt độ dày của đường thành 3

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series
                while (reader.Read())
                {
                    int day = reader.GetInt32("Day");
                    int total = reader.GetInt32("Total");
                    serieasset.Points.AddXY(day, total);
                }

                // Đóng đối tượng MySqlDataReader và đóng kết nối
                reader.Close();


                // Truy vấn dữ liệu từ cơ sở dữ liệu cho RepairsAndMaintenance
                string queryRepairs = @"
    SELECT Alldays.Day, COALESCE(Data.Total, 0) AS Total
    FROM
    (
        SELECT 1 AS Day
        UNION ALL SELECT 2
        UNION ALL SELECT 3
        UNION ALL SELECT 4
        UNION ALL SELECT 5
        UNION ALL SELECT 6
        UNION ALL SELECT 7
        UNION ALL SELECT 8
        UNION ALL SELECT 9
        UNION ALL SELECT 10
        UNION ALL SELECT 11
        UNION ALL SELECT 12
        UNION ALL SELECT 13
        UNION ALL SELECT 14
        UNION ALL SELECT 15
        UNION ALL SELECT 16
        UNION ALL SELECT 17
        UNION ALL SELECT 18
        UNION ALL SELECT 19
        UNION ALL SELECT 20
        UNION ALL SELECT 21
        UNION ALL SELECT 22  
        UNION ALL SELECT 23
        UNION ALL SELECT 24
        UNION ALL SELECT 25
        UNION ALL SELECT 26
        UNION ALL SELECT 27
        UNION ALL SELECT 28
        UNION ALL SELECT 29
        UNION ALL SELECT 30
        UNION ALL SELECT 31  ) AS AllDays
    LEFT JOIN
    (
        SELECT DAY(RepairDate) AS Day, COUNT(*) AS Total 
        FROM repairsandmaintenance
        WHERE YEAR(RepairDate) = @SelectedYear AND MONTH(RepairDate) = @SelectedMonth AND RepairDate IS NOT NULL
        GROUP BY DAY(RepairDate)
    ) AS Data
    ON AllDays.Day = Data.Day;
";
                queryRepairs = queryRepairs.Replace("@SelectedYear", selectedYear.ToString()).Replace("@SelectedMonth", selectedMonth.ToString());

                MySqlDataReader readerRepairs = dbManager.ExecuteQuery(queryRepairs);

                // Tạo một đối tượng Series cho RepairsAndMaintenance
                Series seriesRepairs = new Series();
                seriesRepairs.ChartType = SeriesChartType.Line;
                seriesRepairs.BorderWidth = 3;
                seriesRepairs.MarkerStyle = MarkerStyle.Circle;
                seriesRepairs.MarkerSize = 10;
                seriesRepairs.Name = "Repairs";

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series cho RepairsAndMaintenance
                while (readerRepairs.Read())
                {
                    int monthRepairs = readerRepairs.GetInt32("Day");
                    int totalRepairs = readerRepairs.GetInt32("Total");
                    seriesRepairs.Points.AddXY(monthRepairs, totalRepairs);
                }

                // Đóng đối tượng MySqlDataReader cho RepairsAndMaintenance
                readerRepairs.Close();
                // Truy vấn dữ liệu từ cơ sở dữ liệu cho TransferDate
                string queryTransfers = @"
    SELECT Alldays.Day, COALESCE(Data.Total, 0) AS Total
    FROM
    (
        SELECT 1 AS Day
        UNION ALL SELECT 2
        UNION ALL SELECT 3
        UNION ALL SELECT 4
        UNION ALL SELECT 5
        UNION ALL SELECT 6
        UNION ALL SELECT 7
        UNION ALL SELECT 8
        UNION ALL SELECT 9
        UNION ALL SELECT 10
        UNION ALL SELECT 11
        UNION ALL SELECT 12
        UNION ALL SELECT 13
        UNION ALL SELECT 14
        UNION ALL SELECT 15
        UNION ALL SELECT 16
        UNION ALL SELECT 17
        UNION ALL SELECT 18
        UNION ALL SELECT 19
        UNION ALL SELECT 20
        UNION ALL SELECT 21
        UNION ALL SELECT 22  
        UNION ALL SELECT 23
        UNION ALL SELECT 24
        UNION ALL SELECT 25
        UNION ALL SELECT 26
        UNION ALL SELECT 27
        UNION ALL SELECT 28
        UNION ALL SELECT 29
        UNION ALL SELECT 30
        UNION ALL SELECT 31  ) AS AllDays
    LEFT JOIN
    (
        SELECT DAY(TransferDate) AS Day, COUNT(*) AS Total 
        FROM historytransferasset
        WHERE YEAR(TransferDate) = @SelectedYear AND MONTH(TransferDate) = @SelectedMonth AND TransferDate IS NOT NULL
        GROUP BY DAY(TransferDate)
    ) AS Data
    ON AllDays.Day = Data.Day;
";
                queryTransfers = queryTransfers.Replace("@SelectedYear", selectedYear.ToString()).Replace("@SelectedMonth", selectedMonth.ToString());

                MySqlDataReader readerTransfers = dbManager.ExecuteQuery(queryTransfers);

                // Tạo một đối tượng Series cho TransferDate
                Series seriesTransfers = new Series();
                seriesTransfers.ChartType = SeriesChartType.Line;
                seriesTransfers.BorderWidth = 3;
                seriesTransfers.MarkerStyle = MarkerStyle.Circle;
                seriesTransfers.MarkerSize = 10;
                seriesTransfers.Name = "Transfers";

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series cho TransferDate
                while (readerTransfers.Read())
                {
                    int monthTransfers = readerTransfers.GetInt32("Day");
                    int totalTransfers = readerTransfers.GetInt32("Total");
                    seriesTransfers.Points.AddXY(monthTransfers, totalTransfers);
                }

                // Đóng đối tượng MySqlDataReader cho TransferDate
                readerTransfers.Close();
                // Truy vấn dữ liệu từ cơ sở dữ liệu cho DisposalDate
                string queryDisposal = @"
    SELECT Alldays.Day, COALESCE(Data.Total, 0) AS Total
    FROM
    (
        SELECT 1 AS Day
        UNION ALL SELECT 2
        UNION ALL SELECT 3
        UNION ALL SELECT 4
        UNION ALL SELECT 5
        UNION ALL SELECT 6
        UNION ALL SELECT 7
        UNION ALL SELECT 8
        UNION ALL SELECT 9
        UNION ALL SELECT 10
        UNION ALL SELECT 11
        UNION ALL SELECT 12
        UNION ALL SELECT 13
        UNION ALL SELECT 14
        UNION ALL SELECT 15
        UNION ALL SELECT 16
        UNION ALL SELECT 17
        UNION ALL SELECT 18
        UNION ALL SELECT 19
        UNION ALL SELECT 20
        UNION ALL SELECT 21
        UNION ALL SELECT 22  
        UNION ALL SELECT 23
        UNION ALL SELECT 24
        UNION ALL SELECT 25
        UNION ALL SELECT 26
        UNION ALL SELECT 27
        UNION ALL SELECT 28
        UNION ALL SELECT 29
        UNION ALL SELECT 30
        UNION ALL SELECT 31  ) AS AllDays
    LEFT JOIN
    (
        SELECT DAY(DisposalDate) AS Day, COUNT(*) AS Total 
        FROM disposal
        WHERE YEAR(DisposalDate) = @SelectedYear AND MONTH(DisposalDate) = @SelectedMonth AND DisposalDate IS NOT NULL
        GROUP BY DAY(DisposalDate)
    ) AS Data
    ON AllDays.Day = Data.Day;
";
                queryDisposal = queryDisposal.Replace("@SelectedYear", selectedYear.ToString()).Replace("@SelectedMonth", selectedMonth.ToString());

                MySqlDataReader readerDisposal = dbManager.ExecuteQuery(queryDisposal);

                // Tạo một đối tượng Series cho DisposalDate
                Series seriesDisposal = new Series();
                seriesDisposal.ChartType = SeriesChartType.Line;
                seriesDisposal.BorderWidth = 3;
                seriesDisposal.MarkerStyle = MarkerStyle.Circle;
                seriesDisposal.MarkerSize = 10;
                seriesDisposal.Color = Color.Purple; // Chọn màu cho đường line
                seriesDisposal.Name = "Disposals";

                // Đọc dữ liệu từ MySqlDataReader và thêm vào Series cho DisposalDate
                while (readerDisposal.Read())
                {
                    int monthDisposal = readerDisposal.GetInt32("Day");
                    int totalDisposal = readerDisposal.GetInt32("Total");
                    seriesDisposal.Points.AddXY(monthDisposal, totalDisposal);
                }

                // Đóng đối tượng MySqlDataReader cho DisposalDate
                readerDisposal.Close();



                dbManager.CloseConnection();

                // Thêm Series vào chart1
                chart1.Series.Add(serieasset);
                // Thêm Series mới vào biểu đồ
                chart1.Series.Add(seriesRepairs);
                // Thêm Series mới vào biểu đồ
                chart1.Series.Add(seriesTransfers);
                // Loại bỏ cột đầu và cột cuối
                // Thêm Series mới vào biểu đồ
                chart1.Series.Add(seriesDisposal);
                // Định nghĩa các màu pastel
                Color pastelOrange = Color.FromArgb(255, 255, 192, 128); // Màu cam
                Color pastelBlue = Color.FromArgb(255, 128, 192, 255);   // Màu lam
                Color pastelGreen = Color.FromArgb(255, 128, 255, 128);  // Màu lục
                Color pastelPurple = Color.FromArgb(255, 192, 128, 255); // Màu tím
                chart1.Series[0].Name = "Purchases";

                // Đặt màu cho các series trong biểu đồ
                chart1.Series["Purchases"].Color = pastelOrange;
                chart1.Series["Repairs"].Color = pastelBlue;
                chart1.Series["Transfers"].Color = pastelGreen;
                chart1.Series["Disposals"].Color = pastelPurple;

                chart1.ChartAreas[0].AxisY.Minimum = 0; // Giá trị tối thiểu bạn muốn cố định
                chart1.ChartAreas[0].AxisY.Maximum = 10;
                chart1.ChartAreas[0].AxisX.Minimum = 1;
                chart1.ChartAreas[0].AxisX.Maximum = 31;
                chart1.ChartAreas[0].AxisX.Title = "Day";
                chart1.ChartAreas[0].AxisY.Title = "Asset";
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisX.Interval = 1;

                // Đặt sự kiện MouseMove cho từng series
                chart1.MouseMove += Chart2_MouseMove;


            }
            else
            {
                // Xử lý khi không thể mở kết nối
                MessageBox.Show("Could not open connection to database.");
            }
        }
        private void Chart2_MouseMove(object sender, MouseEventArgs e)
        {
            // Lấy thông tin vị trí con trỏ chuột trên biểu đồ
            HitTestResult result = chart1.HitTest(e.X, e.Y);

            // Kiểm tra xem con trỏ chuột có đang nằm trên một điểm dữ liệu của series nào không
            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                // Nếu con trỏ chuột đang nằm trên một điểm dữ liệu
                Series series = result.Series;
                DataPoint dataPoint = series.Points[result.PointIndex];
                int month = (int)dataPoint.XValue;
                int total = (int)dataPoint.YValues[0];

                // Đặt màu cho series đó
                series.Color = Color.Red; // Đặt màu sắc bạn muốn ở đây

                // Đẩy series đó lên trên cùng
                chart1.Series.Remove(series);
                chart1.Series.Insert(3, series);

                // Hiển thị tooltip
                ShowToolTip($" {month}\nTotal: {total}", e.Location);
            }
            else
            {
                // Nếu con trỏ chuột không nằm trên một điểm dữ liệu của series nào
                // Ẩn tooltip
                HideToolTip();
                Color pastelOrange = Color.FromArgb(255, 255, 192, 128); // Màu cam
                Color pastelBlue = Color.FromArgb(255, 128, 192, 255);   // Màu lam
                Color pastelGreen = Color.FromArgb(255, 128, 255, 128);  // Màu lục
                Color pastelPurple = Color.FromArgb(255, 192, 128, 255); // Màu tím

                // Đặt màu cho các series trong biểu đồ
                chart1.Series["Purchases"].Color = pastelOrange;
                chart1.Series["Repairs"].Color = pastelBlue;
                chart1.Series["Transfers"].Color = pastelGreen;
                chart1.Series["Disposals"].Color = pastelPurple;
            }
        }


        private void ShowToolTip(string text, Point location)
        {
            if (currentToolTip == null)
            {
                currentToolTip = new ToolTip();
            }

            currentToolTip.Show(text, chart1, location.X + 15, location.Y + 15);
        }

        private void HideToolTip()
        {
            if (currentToolTip != null)
            {
                currentToolTip.Hide(chart1);
            }
        }

        private void ClearChart()
        {
            // Xóa tất cả các series trong biểu đồ
            chart1.Series.Clear();

            // Xóa tất cả các dữ liệu (points) trong các series
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }

            // Xóa tất cả các khu vực biểu đồ
            chart1.ChartAreas.Clear();

            // Tạo một khu vực biểu đồ mới
            chart1.ChartAreas.Add(new ChartArea());

            // Gọi Dispose() để giải phóng tài nguyên của biểu đồ
            //chart1.Dispose();
        }
        private void button13_Click(object sender, EventArgs e)
        {
            selectedYear = DateTime.Now.Year;
            ClearChart();
            DrawLineChart();

        }
        private void LoadRecentYears()
        {
            // Lấy năm hiện tại
            int currentYear = DateTime.Now.Year;

            // Thêm 5 năm gần đây vào ComboBox
            for (int i = currentYear; i > currentYear - 5; i--)
            {
                comboBox1.Items.Add(i);
            }

            // Chọn năm hiện tại là mặc định
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedYear = (int)comboBox1.SelectedItem;
            ClearChart();
            DrawLineChart();
        }
        private void LoadMonthsIntoComboBox()
        {
            // Xóa các mục cũ trong ComboBox trước khi thêm mới
            comboBox2.Items.Clear();

            // Thêm các tháng vào ComboBox
            for (int i = 1; i <= 12; i++)
            {
                comboBox2.Items.Add(i);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMonth = (int)comboBox2.SelectedItem;
            selectedYear = (int)comboBox1.SelectedItem;
            ClearChart();
            DrawLineChart2();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectedMonth = DateTime.Now.Month;
            selectedYear = DateTime.Now.Year;
            ClearChart();
            DrawLineChart2();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
