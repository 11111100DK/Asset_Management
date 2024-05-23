using AsserManagement.GUI;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace AsserManagement
{
    public partial class Form1 : Form
    {
        private DatabaseManager dbManager;
        private User loggedInUser;
        private bool isDragging;
        private Point lastCursorPosition;
        private bool isPanelVisible = false; // Biến cờ để kiểm tra trạng thái hiển thị của FlowLayoutPanel

        public Form1()
        {
            InitializeComponent();
            dbManager = new DatabaseManager();

            this.StartPosition = FormStartPosition.CenterScreen;

            this.MouseDown += MyForm_MouseDown;
            this.MouseMove += MyForm_MouseMove;
            this.MouseUp += MyForm_MouseUp;
            Home();
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel1.AutoScroll = true;
            Notice();


        }
        public void Notice()
        {
            int i=0;
            flowLayoutPanel1.Controls.Clear();
            // Mở kết nối đến cơ sở dữ liệu
            if (dbManager.OpenConnection())
            {
                // Lấy ngày hiện tại
                DateTime currentDate = DateTime.Now;

                // Chuyển đổi ngày hiện tại sang chuỗi có định dạng YYYY-MM-DD để so sánh với cơ sở dữ liệu
                string currentDateStr = currentDate.ToString("yyyy-MM-dd");

                // Xây dựng câu truy vấn SQL với điều kiện WarrantyDate là 7 ngày gần đây từ ngày hiện tại và sắp xếp theo WarrantyDate
                string query = $"SELECT AssetName, WarrantyDate, Image FROM fixedassets WHERE (WarrantyDate >= '{currentDateStr}' AND WarrantyDate <= DATE_ADD('{currentDateStr}', INTERVAL 14 DAY)) OR WarrantyDate < '{currentDateStr}' ORDER BY WarrantyDate";

                // Thực hiện truy vấn để lấy dữ liệu
                MySqlDataReader reader = dbManager.ExecuteQuery(query);

                // Đổ dữ liệu từ cơ sở dữ liệu ra ListBox
                while (reader.Read())
                {
                    // Lấy dữ liệu từ cột AssetName, WarrantyDate và Image
                    string name = reader["AssetName"].ToString();
                    string warrantyDate = reader["WarrantyDate"].ToString();
                    string imageData = reader["Image"].ToString();
                    string imagePath = Path.Combine(Application.StartupPath, "Images", imageData);
                    // Chuyển đổi dữ liệu hình ảnh từ mảng byte sang hình ảnh
                   

                    // Tạo một thể hiện mới của UserControl
                    var userControl = new Notice();
                    userControl.Click += UserControl_Click;

                    // Thiết lập giá trị của các label và PictureBox trong UserControl dựa trên dữ liệu từ cơ sở dữ liệu
                    userControl.SetData(name, warrantyDate);
                    userControl.SetImage(imagePath);

                    // Thêm UserControl vào FlowLayoutPanel
                    flowLayoutPanel1.Controls.Add(userControl);
                    i++;
                }

                // Đóng kết nối đến cơ sở dữ liệu sau khi hoàn thành
                dbManager.CloseConnection();
            }
            else
            {
                // Xử lý khi không thể mở kết nối đến cơ sở dữ liệu
            }
            label5.Text = i.ToString();
        }

        public Form1(User user) : this()
        {
            loggedInUser = user;
            DisplayUserData();

        }
        private void UserControl_Click(object sender, EventArgs e)
        {
            // Đặt màu nền của tất cả các UserControl trong FlowLayoutPanel về màu mặc định
            foreach (Notice control in flowLayoutPanel1.Controls)
            {
                control.BackColor = DefaultBackColor;
            }

            // Đặt màu nền của UserControl được click thành một màu khác để biểu thị việc chọn
            button5_Click(sender, e);
            flowLayoutPanel1.Visible = false;
            isPanelVisible = false;
            label5.Visible = true;
            // Thực hiện các hành động khác tùy thuộc vào UserControl được chọn
        }
        public void DisplayUserData()
        {
            //labelUserId.Text = $"User ID: {loggedInUser.UserId}";
            label3.Text = $"{loggedInUser.Username}";
                string image = loggedInUser.Image;

            //labelEmail.Text = $"Email: {loggedInUser.Email}";
            //labelRole.Text = $"Role: {loggedInUser.Role}";
            //labelCompanyID.Text = $"Company ID: {loggedInUser.CompanyID}";
            //string imagePath = Path.Combine(Application.StartupPath, "Images", image);
            //pictureBox1.ImageLocation = imagePath;
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            string imagePath2 = Path.Combine(Application.StartupPath, "Images", image);
            pictureBox3.ImageLocation = imagePath2;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddEllipse(0, 0, pictureBox3.Width, pictureBox3.Height);
            pictureBox3.Region = new Region(graphicsPath);

            string companyName = "";
            string address = "";
            string contactInfo = "";
            string email = "";
            string website = "";
            string logoFileName = ""; // Để lưu tên file logo

            string getCompanyInfoQuery = $"SELECT * FROM companyinformation WHERE CompanyID = {loggedInUser.CompanyID}";

            try
            {
                dbManager.OpenConnection();
                using (MySqlDataReader reader = dbManager.ExecuteQuery(getCompanyInfoQuery))
                {
                    if (reader.Read())
                    {
                        companyName = reader["CompanyName"].ToString();
                        address = reader["Address"].ToString();
                        contactInfo = reader["ContactInformation"].ToString();
                        email = reader["Email"].ToString();
                        website = reader["Website"].ToString();
                        // Đọc tên file logo
                        logoFileName = reader["Logo"].ToString();
                        // Các thông tin khác cần thiết
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ khi thực hiện truy vấn
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            // Hiển thị thông tin của người dùng và công ty trên các controls
            label3.Text = loggedInUser.Username;
            label2.Text = companyName;
            // Hiển thị logo (nếu có)
            if (!string.IsNullOrEmpty(logoFileName))
            {
                string imagePath = Path.Combine(Application.StartupPath, "Images", logoFileName);
                pictureBox1.ImageLocation = imagePath;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }



        }
        // Phương thức để load lại dữ liệu trên Form1
        public void ReloadFormData()
        {

            // Thực hiện load lại dữ liệu trên Form1
            DisplayUserData();
            // Giả sử DisplayUserData là phương thức hiển thị dữ liệu trên Form1
        }

        private void Home()
        {
            // Tạo một thể hiện mới của UserControl
            GUIHome gUIHome = new GUIHome();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUIHome.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUIHome);
            ActivateButton((Button)button1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUIAsset gUIAsset = new GUIAsset();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUIAsset.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUIAsset); ;
            ActivateButton((Button)sender); // Pass the clicked button
        }

       

        private void button1_Click(object sender, EventArgs e)
        {
            Home();
            ActivateButton((Button)sender); // Pass the clicked button
        }
        private void ActivateButton(Button senderBtn) // Change parameter type to Button
        {
            if (senderBtn != null)
            {
                DisableButon();
                senderBtn.BackColor = Color.FromArgb(209, 230, 200);
                senderBtn.ForeColor = Color.FromArgb(64, 64, 64);

                senderBtn.FlatAppearance.BorderColor = senderBtn.BackColor;

            }
        }

        private void DisableButon()
        {
            foreach (Control control in panel1.Controls)
            {
                if (control is Button)
                {
                    ((Button)control).BackColor = Color.FromArgb(174, 210, 159);
                    ((Button)control).ForeColor = Color.White;

                    ((Button)control).FlatAppearance.BorderColor = control.BackColor;

                }
            }
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUIInfo gUIInfo = new GUIInfo();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUIInfo.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUIInfo);
            ActivateButton((Button)sender); // Pass the clicked button

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUITransfer gUITransfer = new GUITransfer();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUITransfer.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUITransfer);
            ActivateButton((Button)sender); // Pass the clicked button
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUIRepair gUIRepair = new GUIRepair();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUIRepair.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUIRepair);
            ActivateButton(button5); // Pass the clicked button
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUIDisposal gUIDisposal = new GUIDisposal();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUIDisposal.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUIDisposal);
            ActivateButton((Button)sender); // Pass the clicked button
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUIStactic gUIStactic = new GUIStactic();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUIStactic.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUIStactic);
            ActivateButton((Button)sender); // Pass the clicked button
        }
        private void button8_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUICompany gUICompany = new GUICompany(loggedInUser,this);

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUICompany.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUICompany);
            ActivateButton((Button)sender); // Pass the clicked button
        }
        
        private void MyForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Khi nhấn chuột trái, bắt đầu di chuyển Form
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursorPosition = e.Location;
            }
        }

        private void MyForm_MouseMove(object sender, MouseEventArgs e)
        {
            // Khi đang di chuyển, cập nhật vị trí mới cho Form
            if (isDragging)
            {
                this.Left += e.X - lastCursorPosition.X;
                this.Top += e.Y - lastCursorPosition.Y;
            }
        }

        private void MyForm_MouseUp(object sender, MouseEventArgs e)
        {
            // Khi thả chuột, dừng di chuyển
            isDragging = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (isPanelVisible)
            {
                // Nếu FlowLayoutPanel đang hiển thị, ẩn nó đi và cập nhật trạng thái của biến cờ
                flowLayoutPanel1.Visible = false;
                isPanelVisible = false;
                label5.Visible = true  ;
            }
            else
            {
                Notice();
                // Nếu FlowLayoutPanel đang ẩn, hiển thị nó lên và cập nhật trạng thái của biến cờ
                flowLayoutPanel1.Location = new Point(830, 45);

                flowLayoutPanel1.Visible = true;
                isPanelVisible = true;
                flowLayoutPanel1.BringToFront();
                label5.Visible = false;

            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện mới của UserControl
            GUISetting gUISetting = new GUISetting();

            // Đặt Dock của UserControl là Fill để nó lấp đầy Panel2
            gUISetting.Dock = DockStyle.Fill;

            // Xóa tất cả các điều khiển hiện có trong Panel2 (nếu có)
            panel2.Controls.Clear();

            // Thêm UserControl vào Panel2
            panel2.Controls.Add(gUISetting);
            ActivateButton((Button)sender); // Pass the clicked button
        }

    }
}
