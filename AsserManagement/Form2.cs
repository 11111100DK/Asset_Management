using DocumentFormat.OpenXml.Wordprocessing;
using Google.Apis.Admin.Directory.directory_v1.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AsserManagement
{
    public partial class Form2 : Form
    {
        private DatabaseManager dbManager;
        private bool isDragging;
        private Point lastCursorPosition;
        private int x;
        private int y;

        public Form2()
        {
            InitializeComponent();
            InitializeCustomComponents();
            dbManager = new DatabaseManager();

            this.StartPosition = FormStartPosition.CenterScreen;

            this.MouseDown += MyForm_MouseDown;
            this.MouseMove += MyForm_MouseMove;
            this.MouseUp += MyForm_MouseUp;
            


        }

        // Phương thức xử lý sự kiện ItemClick cho UserControl

        


        private void button1_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Authenticate user
            User user = AuthenticateUser(username, password);
            if (user != null)
            {
                // Open Form1 and pass logged-in user data
                Form1 form1 = new Form1(user);
                GUICompany guiCompany = new GUICompany(user,form1);
                form1.Controls.Add(guiCompany);


                form1.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Username hoặc Password không đúng.");
            }
        }

        private User AuthenticateUser(string username, string password)
        {
            // Query database to authenticate user
            string query = $"SELECT * FROM users WHERE Username = '{username}' AND Password = '{password}'";
            if (dbManager.OpenConnection())
            {
                var reader = dbManager.ExecuteQuery(query);
                if (reader.Read())
                {
                    User user = new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        Email = reader["Email"].ToString(),
                        Role = Convert.ToInt32(reader["Role"]),
                        CompanyID = Convert.ToInt32(reader["CompanyID"]),
                        Image = reader["Image"].ToString(),

                    };
                    dbManager.CloseConnection();
                    return user;
                }
                dbManager.CloseConnection();
            }
            return null;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
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
        private void InitializeCustomComponents()
        {
            // Tạo nút minimize
            Button minimizeButton = new Button();
            minimizeButton.Text = "-";
            minimizeButton.Size = new Size(25, 25);
            minimizeButton.Location = new Point(this.Width - 50, 0);
            minimizeButton.Click += (sender, e) => this.WindowState = FormWindowState.Minimized;
            this.Controls.Add(minimizeButton);

            // Tạo nút close
            Button closeButton = new Button();
            closeButton.Text = "X";
            closeButton.Size = new Size(25, 25);
            closeButton.Location = new Point(this.Width - 25, 0);
            closeButton.Click += (sender, e) => this.Close();
            this.Controls.Add(closeButton);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}


