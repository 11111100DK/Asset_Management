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

namespace AsserManagement
{
    public partial class GUIHome : UserControl
    {
        private DatabaseManager dbManager;

        public GUIHome()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
            Label();

        }
        public void Label()
        {
            if (dbManager.OpenConnection())
            {
                // Truy vấn để tính tổng số hàng từ các bảng
                string query1 = "SELECT COUNT(*) FROM departments";
                string query2 = "SELECT COUNT(*) FROM employees";
                string query3 = "SELECT COUNT(*) FROM assettypes";
                string query4 = "SELECT COUNT(*) FROM fixedassets";

                // Thực hiện truy vấn và lấy tổng số hàng từ mỗi bảng
                // Khai báo biến đếm và Timer cho mỗi Label
                int count1 = GetRowCount(query1);
                int count2 = GetRowCount(query2);
                int count3 = GetRowCount(query3);
                int count4 = GetRowCount(query4);
                int currentCount1 = 0;
                int currentCount2 = 0;
                int currentCount3 = 0;
                int currentCount4 = 0;
                Timer timer1 = new Timer();
                Timer timer2 = new Timer();
                Timer timer3 = new Timer();
                Timer timer4 = new Timer();

                // Thiết lập thời gian cho Timer (ví dụ: mỗi 100ms)
                int interval = 100;

                // Thiết lập sự kiện cho mỗi Timer
                timer1.Tick += (sender, e) => UpdateLabel(sender, e, count1, ref currentCount1, label2);
                timer2.Tick += (sender, e) => UpdateLabel(sender, e, count2, ref currentCount2, label3);
                timer3.Tick += (sender, e) => UpdateLabel(sender, e, count3, ref currentCount3, label4);
                timer4.Tick += (sender, e) => UpdateLabel(sender, e, count4, ref currentCount4, label5);

                // Bắt đầu Timer
                timer1.Interval = interval;
                timer2.Interval = interval;
                timer3.Interval = interval;
                timer4.Interval = interval;
                timer1.Start();
                timer2.Start();
                timer3.Start();
                timer4.Start();

                // Đóng kết nối đến cơ sở dữ liệu
                dbManager.CloseConnection();
            }
            else
            {
                // Xử lý khi không thể mở kết nối đến cơ sở dữ liệu
            }
        }
        void UpdateLabel(object sender, EventArgs e, int count, ref int currentCount, Label label)
        {
            // Tăng giá trị hiện tại của Label
            currentCount++;

            // Hiển thị giá trị hiện tại của Label
            label.Text = currentCount.ToString();

            // Kiểm tra nếu giá trị hiện tại đã đạt đến giá trị count
            if (currentCount >= count)
            {
                // Dừng Timer
                ((Timer)sender).Stop();
            }
        }
        int GetRowCount(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count;
        }
    }
}
