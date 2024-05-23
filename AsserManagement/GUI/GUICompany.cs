using AsserManagement.BUS;
using AsserManagement.DAO;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace AsserManagement
{
    public partial class GUICompany : UserControl
    {
        private BUSCompany bUSCompany;            
        string id1;
        private User loggedInUser;
        private DatabaseManager dbManager;
        private string selectedImagePath;
        private Form1 parentForm;

        private string selectedImagePath1;
        private string selectedImagePath2;



        public GUICompany(User user, Form1 parentForm)
        {
            InitializeComponent();
            dbManager = new DatabaseManager();
            this.parentForm = parentForm;

            bUSCompany = new BUSCompany();
            LoadSuppliers();
            loggedInUser = user;

            DisplayUserData();

        }
        
        

        private void DisplayUserData()
        {

            //labelUserId.Text = $"User ID: {loggedInUser.UserId}";
            //label3.Text = $"{loggedInUser.Username}";
            string image = loggedInUser.Image;

            maskedTextBox9.Text = $"{loggedInUser.Email}";
            //labelRole.Text = $"Role: {loggedInUser.Role}";
            //labelCompanyID.Text = $"Company ID: {loggedInUser.CompanyID}";
            //string imagePath = Path.Combine(Application.StartupPath, "Images", image);
            //pictureBox1.ImageLocation = imagePath;
            //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            string imagePath2 = Path.Combine(Application.StartupPath, "Images", image);
            pictureBox2.ImageLocation = imagePath2;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            if (File.Exists(imagePath2))
            {
                openFileDialog2.FileName = imagePath2;
                selectedImagePath2 = imagePath2;
            }
            else
            {
                openFileDialog1.FileName = string.Empty;
            }
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
            //label3.Text = loggedInUser.Username;
            maskedTextBox4.Text = companyName;
            maskedTextBox5.Text = address;
            maskedTextBox6.Text = contactInfo;
            maskedTextBox7.Text = email;
            maskedTextBox8.Text = website;

            // Hiển thị logo (nếu có)
            if (!string.IsNullOrEmpty(logoFileName))
            {
                string imagePath = Path.Combine(Application.StartupPath, "Images", logoFileName);
                pictureBox1.ImageLocation = imagePath;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                // Đảm bảo rằng OpenFileDialog chứa đúng đường dẫn của hình ảnh
                if (File.Exists(imagePath))
                {
                    openFileDialog1.FileName = imagePath;
                    selectedImagePath1 = imagePath;
                }
                else
                {
                    openFileDialog1.FileName = string.Empty;
                }
            }



        }

        private void LoadSuppliers()
        {
            dataGridView1.DataSource = bUSCompany.GetAllSuppliers();
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[1].Width = 180;
            dataGridView1.Columns[2].Width = 250;
            dataGridView1.Columns[3].Width = 120;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = maskedTextBox1.Text;
            string address = maskedTextBox2.Text;
            string contact = maskedTextBox3.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(contact))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một TextBox rỗng
            }
            bool result = bUSCompany.AddSupplier(name, address, contact);
            if (result)
            {
                MessageBox.Show("Asset type added successfully.");
                ClearFields1();
            }
            else
            {
                MessageBox.Show("Failed to add asset type.");
            }
            LoadSuppliers();
        }
        private void ClearFields1()
        {
            maskedTextBox1.Text = "";
            maskedTextBox2.Text = "";
            maskedTextBox3.Text = "";

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn một hàng không
            if (e.RowIndex >= 0)
            {
                // Lấy giá trị của cột ID trong hàng được chọn
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                id1 = selectedRow.Cells["ID"].Value.ToString(); // Thay "IDColumnName" bằng tên cột ID trong DataGridView


                // Lấy thông tin AssetType từ BUS
                Supplier supplier = bUSCompany.GetSupplierById(id1);

                if (supplier != null)
                {
                    // Hiển thị thông tin trên giao diện
                    maskedTextBox1.Text = supplier.Name;
                    maskedTextBox2.Text = supplier.Address;
                    maskedTextBox3.Text = supplier.ContactInformation;

                    button8.Visible = true;
                    button9.Visible = true;
                    button2.Visible = false;
                    button12.Visible = true;

                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin cho ID đã chọn.");
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            id1 = string.Empty;
            maskedTextBox1.Text = "";
            maskedTextBox2.Text = "";
            maskedTextBox3.Text = "";
            button8.Visible = false;
            button9.Visible = false;
            button2.Visible = true;
            button12.Visible = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string newName = maskedTextBox1.Text;
            string newAddress = maskedTextBox2.Text;
            string newContact = maskedTextBox3.Text;

            bool success = bUSCompany.UpdateSupplier(id1, newName, newAddress, newContact);

            if (success)
            {
                MessageBox.Show("Cập nhật thông tin thành công.");
                LoadSuppliers();
                button12_Click(sender, e);

            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin.");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Kiểm tra kết quả của hộp thoại
            if (result == DialogResult.Yes)
            {
                // Xóa bản ghi từ ID
                if (bUSCompany.DeleteSupplier(id1))
                {
                    MessageBox.Show("Xóa thông tin thành công.");
                    LoadSuppliers();
                    button12_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Không thể xóa bản ghi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private enum OpenFileDialogType
        {
            OpenFileDialog1,
            OpenFileDialog2
        }
        private string OpenImageFileDialog(OpenFileDialogType dialogType)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.png;*.gif)|*.bmp;*.jpg;*.png;*.gif|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Hiển thị hình ảnh trong PictureBox tương ứng
                if (dialogType == OpenFileDialogType.OpenFileDialog1)
                {
                    selectedImagePath1 = openFileDialog.FileName;
                    pictureBox1.ImageLocation = selectedImagePath1;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    return selectedImagePath1;

                }
                else if (dialogType == OpenFileDialogType.OpenFileDialog2)
                {
                    selectedImagePath2 = openFileDialog.FileName;
                    pictureBox2.ImageLocation = selectedImagePath2;
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    return selectedImagePath2;

                }
            }

            return selectedImagePath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string imagePath1 = OpenImageFileDialog(OpenFileDialogType.OpenFileDialog1);

            Console.WriteLine(pictureBox1.ImageLocation);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string imagePath2 = OpenImageFileDialog(OpenFileDialogType.OpenFileDialog2);

            Console.WriteLine(pictureBox2.ImageLocation);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedImagePath1))
            {
                // Lấy dữ liệu từ các controls trên form
                string name = maskedTextBox4.Text;              
                string address = maskedTextBox5.Text;
                string phone = maskedTextBox6.Text;
                string email = maskedTextBox7.Text;
                string website = maskedTextBox8.Text;

                // Thực hiện truy vấn cập nhật dữ liệu
                if (dbManager.OpenConnection())
                {
                    // Lấy tên file hình ảnh
                    string imageName = Path.GetFileName(selectedImagePath1);
                    string destinationDirectory = Path.Combine(Application.StartupPath, "Images");
                    string destinationPath = Path.Combine(destinationDirectory, imageName);
                    if (!File.Exists(destinationPath))
                    {
                        // Nếu tệp không tồn tại, thực hiện sao chép
                        File.Copy(selectedImagePath1, destinationPath, true);
                    }                    // Đường dẫn tạm thời để lưu trữ hình ảnh
                    string tempImagePath = Path.Combine(Path.GetTempPath(), imageName);

                    // Sao chép tệp tin hình ảnh vào đường dẫn tạm thời
                    File.Copy(selectedImagePath1, tempImagePath, true);

                    // Thiết lập đường dẫn cho OpenFileDialog
                    openFileDialog1.FileName = tempImagePath;

                    string query =  $"UPDATE companyinformation SET CompanyName = '{name}', " +
                                    $"Address = '{address}', ContactInformation = '{phone}', " +
                                    $"Email = '{email}', Website = '{website}', " +
                                    $"Logo = '{imageName}' " +
                                    $"WHERE CompanyID = {loggedInUser.CompanyID}";


                    MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Sao chép hình ảnh vào thư mục của dự án


                        MessageBox.Show("Dữ liệu đã được cập nhật thành công.");
                        // Xóa dữ liệu từ các controls trên form
                        parentForm.ReloadFormData(); // Gọi phương thức ReloadFormData của Form1 để load lại dữ liệu

                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật dữ liệu.");
                    }
                    dbManager.CloseConnection();
                }
                else
                {
                    MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hình ảnh trước khi cập nhật.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedImagePath2))
            {
                // Lấy dữ liệu từ các controls trên form
                string email = maskedTextBox9.Text;


                // Thực hiện truy vấn cập nhật dữ liệu
                if (dbManager.OpenConnection())
                {
                    // Lấy tên file hình ảnh
                    string imageName = Path.GetFileName(selectedImagePath2);
                    string destinationDirectory = Path.Combine(Application.StartupPath, "Images");
                    string destinationPath = Path.Combine(destinationDirectory, imageName);
                    if (!File.Exists(destinationPath))
                    {
                        // Nếu tệp không tồn tại, thực hiện sao chép
                        File.Copy(selectedImagePath2, destinationPath, true);
                    }                    // Đường dẫn tạm thời để lưu trữ hình ảnh
                    string tempImagePath = Path.Combine(Path.GetTempPath(), imageName);

                    // Sao chép tệp tin hình ảnh vào đường dẫn tạm thời
                    File.Copy(selectedImagePath2, tempImagePath, true);

                    // Thiết lập đường dẫn cho OpenFileDialog
                    openFileDialog2.FileName = tempImagePath;

                    string query = $"UPDATE users SET Email = '{email}', " +                                 
                                    $"Image = '{imageName}' " +
                                    $"WHERE UserId = {loggedInUser.UserId}";


                    MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Sao chép hình ảnh vào thư mục của dự án

                        
                        MessageBox.Show("Dữ liệu đã được cập nhật thành công.");
                        loggedInUser.Email = email; 
                        loggedInUser.Image = imageName;
                        parentForm.ReloadFormData(); // Gọi phương thức ReloadFormData của Form1 để load lại dữ liệu

                        // Xóa dữ liệu từ các controls trên form

                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật dữ liệu.");
                    }
                    dbManager.CloseConnection();
                }
                else
                {
                    MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hình ảnh trước khi cập nhật.");
            }
        }
        private void ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }
            // Kiểm tra xác thực mật khẩu hiện tại
            if (loggedInUser.Password != currentPassword)
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng.");
                return;
            }

            // Kiểm tra xác nhận mật khẩu mới
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Mật khẩu mới và mật khẩu xác nhận không khớp.");
                return;
            }
            // Kiểm tra các trường dữ liệu không null


            // Thực hiện cập nhật mật khẩu mới vào cơ sở dữ liệu
            string query = $"UPDATE users SET Password = '{newPassword}' WHERE UserId = {loggedInUser.UserId}";
            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Đã cập nhật mật khẩu thành công.");
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật mật khẩu không thành công.");
                    }
                    dbManager.CloseConnection();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Lỗi khi cập nhật mật khẩu: " + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string currentPassword = maskedTextBox10.Text;
            string newPassword = maskedTextBox11.Text;
            string confirmPassword = maskedTextBox12.Text;

            ChangePassword(currentPassword, newPassword, confirmPassword);
        }
    }
}
