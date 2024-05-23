using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using AsserManagement.BUS;


namespace AsserManagement
{
    public partial class GUIAsset : UserControl
    {
        private BUSAsset assetBusiness;

        private int pageSize = 10; // Số lượng hàng mỗi trang
        private int currentPage = 1; // Trang hiện tại
        private int totalPages; // Khai báo biến totalPages
        private decimal id;
        private string selectedImagePath;
        private string searchKeyword;
        private int assetTypeID;
        private int departmentID;
        private int employeeID;
        private string status;
        private DateTime FromDate;
        private DateTime ToDate;
        private DateTime FromWarranty;
        private DateTime ToWarranty;
        private int assetID;

        public event EventHandler AddButtonClick;
        private DatabaseManager dbManager;
        
        public GUIAsset()
        {
            InitializeComponent();
            assetBusiness = new BUSAsset();
            dbManager = new DatabaseManager();
            panel1.Visible = false; // Ẩn Panel ban đầu
            dateTimePicker3.Value = new DateTime(2024, 1, 1); // Đặt dateTimePicker3 thành không có giá trị
            dateTimePicker4.Value = new DateTime(2024, 12, 31); // Đặt dateTimePicker4 thành không có giá trị
            dateTimePicker5.Value = new DateTime(2024, 1, 1); // Đặt dateTimePicker5 thành không có giá trị
            dateTimePicker6.Value = new DateTime(2024, 12, 31); // Đặt dateTimePicker6 thành không có giá trị
            panel4.Visible = false; // Ẩn Panel ban đầu

            LoadAssetTypesIntoComboBox();
            LoadDepartmentIntoComboBox();
            LoadEmployeesIntoComboBox();
            AddStaticDataToComboBox();
            LoadSupplierIntoComboBox();
            LoadAssetTypesIntoComboBox2();
            LoadDepartmentIntoComboBox2();
            LoadEmployeesIntoComboBox2();

            button4_Click(null, null);
        }
        private void LoadDataIntoDataGridView(string searchKeyword, int assetTypeID, int departmentID, int employeeID, string status, DateTime FromDate, DateTime ToDate, DateTime FromWarranty, DateTime ToWarranty)
        {
            try
            {
                dataGridView1.DataSource = null; // Bỏ kết nối với dữ liệu
                dataGridView1.Rows.Clear(); // Xóa hết các dòng
                dataGridView1.Columns.Clear(); // Xóa hết các cột
                dataGridView1.CellContentClick -= DataGridView1_CellContentClick; // Hủy đăng ký sự kiện Click cho cột nút "D"
                dataGridView1.CellContentClick -= DataGridView1_CellEditButtonClick; // Hủy đăng ký sự kiện Click cho cột nút "E"
                dataGridView1.CellContentClick -= DataGridView1_CellDeltailButtonClick;
                
                DataTable originalData = assetBusiness.GetAssetData(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
                if (originalData != null)
                {

                    // Tính toán số trang và trang hiện tại
                    totalPages = assetBusiness.GetTotalPages(pageSize, searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);

                    // Lấy dữ liệu cho trang hiện tại
                    DataTable currentPageData = assetBusiness.GetAssetDataForPage(currentPage, pageSize);

                    if (currentPageData != null)
                    {
                        dataGridView1.DataSource = currentPageData;
                        UpdatePaginationControls();

                    }
                    else
                    {
                        MessageBox.Show("Tải dữ liệu thất bại.");
                    }
                }
                else
                {
                    MessageBox.Show("Không thế kết nối cơ sở dữ liệu.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        private void AddColumnsAndEventHandlers()
        {
            // Kiểm tra xem có cột nút đã tồn tại hay không
            if (dataGridView1.Columns["C"] == null)
            {
                // Thêm cột nút vào DataGridView
                DataGridViewImageColumn buttonColumn = new DataGridViewImageColumn();
                buttonColumn.HeaderText = "C"; // Thêm tiêu đề cho cột nút
                buttonColumn.Name = "C";
                // Đặt kiểu dữ liệu của ô là hình ảnh (Image)
                buttonColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                // Sử dụng tài nguyên DelIcon như một hình ảnh Bitmap
                System.Drawing.Bitmap delIcon = AsserManagement.Properties.Resources.p;
                buttonColumn.Image = delIcon;
                dataGridView1.Columns.Add(buttonColumn);
                // Đăng ký sự kiện Click cho cột nút
                dataGridView1.CellContentClick += DataGridView1_CellDeltailButtonClick;
            }
            if (dataGridView1.Columns["E"] == null)
            {
                // Thêm cột nút vào DataGridView
                DataGridViewImageColumn buttonColumn = new DataGridViewImageColumn();
                buttonColumn.HeaderText = "E"; // Thêm tiêu đề cho cột nút
                buttonColumn.Name = "E";
                // Đặt kiểu dữ liệu của ô là hình ảnh (Image)
                buttonColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                // Sử dụng tài nguyên DelIcon như một hình ảnh Bitmap
                System.Drawing.Bitmap delIcon = AsserManagement.Properties.Resources.w;
                buttonColumn.Image = delIcon;
                dataGridView1.Columns.Add(buttonColumn);
                // Đăng ký sự kiện Click cho cột nút
                dataGridView1.CellContentClick += DataGridView1_CellEditButtonClick;
            }            
            if (dataGridView1.Columns["D"] == null)
            {
                // Thêm cột nút vào DataGridView
                DataGridViewImageColumn buttonColumn = new DataGridViewImageColumn();
                buttonColumn.HeaderText = "D"; // Thêm tiêu đề cho cột nút
                buttonColumn.Name = "D";
                // Đặt kiểu dữ liệu của ô là hình ảnh (Image)
                buttonColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
                // Sử dụng tài nguyên DelIcon như một hình ảnh Bitmap
                System.Drawing.Bitmap delIcon = AsserManagement.Properties.Resources.d;
                buttonColumn.Image = delIcon;
                dataGridView1.Columns.Add(buttonColumn);
                // Đăng ký sự kiện Click cho cột nút
                dataGridView1.CellContentClick += DataGridView1_CellContentClick;
            }
            
          
            dataGridView1.Columns[0].Width = 30; 
            dataGridView1.Columns[1].Width = 200; 
            dataGridView1.Columns[2].Width = 100; 
            dataGridView1.Columns[3].Width = 100; 
            dataGridView1.Columns[4].Width = 100;
            dataGridView1.Columns[5].Width = 100; 
            dataGridView1.Columns[6].Width = 100; 
            dataGridView1.Columns[7].Width = 100; 
            dataGridView1.Columns[8].Width = 100; 
            dataGridView1.Columns[9].Width = 30; 
            dataGridView1.Columns[10].Width = 30; 
            dataGridView1.Columns[11].Width = 30; 
        }
        private void DataGridView1_CellDeltailButtonClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["C"].Index)
            {


                try
                {
                    assetID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);

                    if (dbManager.OpenConnection())
                    {
                        string query = $"SELECT f.AssetName AS Name, a.AssetTypeName AS Type, s.SupplierName AS Supplier, d.DepartmentName AS Department, CONCAT(e.LastName, ' ', e.FirstName) AS Employee, f.Value, f.PurchaseDate AS Date, f.WarrantyDate AS Warranty, f.Status, f.Description, f.Image " +
                                       $"FROM fixedassets f " +
                                       $"JOIN assettypes a ON f.AssetTypeID = a.AssetTypeID " +
                                       $"JOIN departments d ON f.DepartmentID = d.DepartmentID " +
                                       $"JOIN suppliers s ON f.SupplierID = s.SupplierID " +
                                       $"JOIN employees e ON f.EmployeeID = e.EmployeeID " +
                                       $"WHERE f.FixedAssetID = {assetID}";

                        MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Lấy dữ liệu từ cơ sở dữ liệu
                            id = assetID;
                            string assetName1 = reader["Name"].ToString();
                            string assetType1 = reader["Type"].ToString();
                            string supplier1 = reader["Supplier"].ToString();
                            string department1 = reader["Department"].ToString();
                            string employee1 = reader["Employee"].ToString();
                            decimal value1 = Convert.ToDecimal(reader["Value"]);
                            DateTime purchaseDate1 = Convert.ToDateTime(reader["Date"]);
                            DateTime warrantyDate1 = Convert.ToDateTime(reader["Warranty"]);
                            string status1 = reader["Status"].ToString();
                            string description1 = reader["Description"].ToString();
                            string imageName1 = reader["Image"].ToString();

                            // Hiển thị dữ liệu trên các controls tương ứng
                            label18.Text = assetName1;
                            label25.Text = value1.ToString();
                            label34.Text = purchaseDate1.ToString("dd-MM-yyyy");
                            label37.Text = warrantyDate1.ToString("dd-MM-yyyy");
                            label27.Text = status1;
                            richTextBox2.Text = description1;
                            label38.Text = department1;
                            label39.Text = employee1;
                            label36.Text = supplier1;
                            label35.Text = assetType1;
                            // Load hình ảnh từ tên tập tin lấy được từ cơ sở dữ liệu
                            string imagePath = Path.Combine(Application.StartupPath, "Images", imageName1);
                            pictureBox2.ImageLocation = imagePath;
                            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

                            // Gán giá trị cho selectedImagePath
                            selectedImagePath = imagePath;
                            panel4.Visible = true;
                            panel4.BringToFront(); // Đảm bảo Panel được vẽ trên tất cả các control khác
                            panel2.Visible = false;
                            panel1.Visible = false;
                            ShowPanelInCenter(panel4, this);
                        }

                        reader.Close();
                        dbManager.CloseConnection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = 0;

            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["D"].Index)
            {
                try
                {
                    dataGridView1.ClearSelection();

                    int assetID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);
                    DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa dữ liệu này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        int rowsAffected = assetBusiness.DeleteAsset(assetID);
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Dữ liệu đã được xóa thành công.");
                            i++;
                            // Cập nhật lại dữ liệu trong DataGridView sau khi xóa thành công
                            LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
                            AddColumnsAndEventHandlers();
                        }
                        else
                        {
                            MessageBox.Show("Không thể xóa dữ liệu.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }     
        }      
        private void DataGridView1_CellEditButtonClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra sự kiện Click có xảy ra trên cột "Edit" không
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["E"].Index)
            {
                try
                {
                    int assetID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["ID"].Value);

                    if (dbManager.OpenConnection())
                    {
                        string query = $"SELECT f.AssetName AS Name, a.AssetTypeName AS Type, s.SupplierName AS Supplier, d.DepartmentName AS Department, CONCAT(e.LastName, ' ', e.FirstName) AS Employee, f.Value, f.PurchaseDate AS Date, f.WarrantyDate AS Warranty, f.Status, f.Description, f.Image " +
                                       $"FROM fixedassets f " +
                                       $"JOIN assettypes a ON f.AssetTypeID = a.AssetTypeID " +
                                       $"JOIN departments d ON f.DepartmentID = d.DepartmentID " +
                                       $"JOIN suppliers s ON f.SupplierID = s.SupplierID " +
                                       $"JOIN employees e ON f.EmployeeID = e.EmployeeID " +
                                       $"WHERE f.FixedAssetID = {assetID}";

                        MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Lấy dữ liệu từ cơ sở dữ liệu
                            id = assetID;
                            string assetName1 = reader["Name"].ToString();
                            string assetType1 = reader["Type"].ToString();
                            string supplier1 = reader["Supplier"].ToString();
                            string department1 = reader["Department"].ToString();
                            string employee1 = reader["Employee"].ToString();
                            decimal value1 = Convert.ToDecimal(reader["Value"]);
                            DateTime purchaseDate1 = Convert.ToDateTime(reader["Date"]);
                            DateTime warrantyDate1 = Convert.ToDateTime(reader["Warranty"]);
                            string status1 = reader["Status"].ToString();
                            string description1 = reader["Description"].ToString();
                            string imageName1 = reader["Image"].ToString();

                            // Hiển thị dữ liệu trên các controls tương ứng
                            textBox1.Text = assetName1;

                            // Tìm và gán giá trị cho ComboBox Type
                            foreach (KeyValuePair<int, string> item in comboBox1.Items)
                            {
                                if (item.Value == assetType1)
                                {
                                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(item);
                                    break;
                                }
                            }
                            foreach (KeyValuePair<int, string> item in comboBox2.Items)
                            {
                                if (item.Value == supplier1)
                                {
                                    comboBox2.SelectedIndex = comboBox2.Items.IndexOf(item);
                                    break;
                                }
                            }

                            foreach (KeyValuePair<int, string> item in comboBox3.Items)
                            {
                                if (item.Value == department1)
                                {
                                    comboBox3.SelectedIndex = comboBox3.Items.IndexOf(item);
                                    break;
                                }
                            }
                            foreach (KeyValuePair<int, string> item in comboBox4.Items)
                            {
                                // Trích xuất phần lastName và firstName từ item.Value
                                string[] parts = item.Value.Split('-');
                                string fullName = parts[1].Trim(); // Loại bỏ dấu cách thừa

                                // So sánh fullName với employee1
                                if (fullName == employee1)
                                {
                                    comboBox4.SelectedItem = item;
                                    break;
                                }
                            }
                            textBox2.Text = value1.ToString();
                            dateTimePicker1.Value = purchaseDate1;
                            dateTimePicker2.Value = warrantyDate1;
                            comboBox5.SelectedItem = status1;
                            richTextBox1.Text = description1;

                            // Load hình ảnh từ tên tập tin lấy được từ cơ sở dữ liệu
                            string imagePath = Path.Combine(Application.StartupPath, "Images", imageName1);
                            pictureBox1.ImageLocation = imagePath;
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                            // Gán giá trị cho selectedImagePath
                            selectedImagePath = imagePath;

                            // Đảm bảo rằng OpenFileDialog chứa đúng đường dẫn của hình ảnh
                            if (File.Exists(imagePath) && id > 0)
                            {
                                openFileDialog.FileName = imagePath;
                                button7_Click(sender, e);
                            }
                            else
                            {
                                openFileDialog.FileName = string.Empty;
                                button7_Click(sender, e);
                            }
                        }

                        reader.Close();
                        dbManager.CloseConnection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void LoadAssetTypesIntoComboBox()
        {
            string query = "SELECT AssetTypeID, AssetTypeName FROM assettypes";
            DataTable dataTable = new DataTable();

            try
            {
                // Mở kết nối tới cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    DataRow zeroRow = dataTable.NewRow();
                    zeroRow["AssetTypeID"] = 0;
                    zeroRow["AssetTypeName"] = ""; // Bạn có thể gán một giá trị mặc định cho tên mục này
                    dataTable.Rows.InsertAt(zeroRow, 0);
                    // Gán dữ liệu vào comboBox1

                    comboBox9.DataSource = dataTable;
                    comboBox9.DisplayMember = "AssetTypeName";
                    comboBox9.ValueMember = "AssetTypeID";
                    comboBox9.SelectedIndex = 0;

                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng kết nối được đóng sau khi sử dụng
                dbManager.CloseConnection();
            }

        }
        private void LoadAssetTypesIntoComboBox2()
        {
            string query = "SELECT AssetTypeID, AssetTypeName FROM assettypes";
            DataTable dataTable = new DataTable();

            try
            {
                // Mở kết nối tới cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);

                    // Tạo danh sách KeyValuePair để chứa các cặp giá trị DepartmentID và DepartmentName
                    List<KeyValuePair<int, string>> assettypeList = new List<KeyValuePair<int, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int assettypeID = Convert.ToInt32(row["AssettypeID"]);
                        string assettypeName = row["AssettypeName"].ToString();
                        assettypeList.Add(new KeyValuePair<int, string>(assettypeID, assettypeName));
                    }

                    // Chỉ định DisplayMember là "Value" để hiển thị Name
                    comboBox1.DisplayMember = "Value";
                    comboBox1.ValueMember = "Key";
                    comboBox1.DataSource = assettypeList;

                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng kết nối được đóng sau khi sử dụng
                dbManager.CloseConnection();
            }
            
        }

        private void LoadSupplierIntoComboBox()
        {
            string query = "SELECT SupplierID, SupplierName FROM suppliers";
            DataTable dataTable = new DataTable();

            try
            {
                // Mở kết nối tới cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);

                    // Tạo một danh sách KeyValuePair để lưu trữ dữ liệu
                    List<KeyValuePair<int, string>> supplierList = new List<KeyValuePair<int, string>>();

                    // Duyệt qua mỗi dòng trong DataTable và thêm vào danh sách KeyValuePair
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int supplierID = Convert.ToInt32(row["SupplierID"]);
                        string supplierName = row["SupplierName"].ToString();
                        supplierList.Add(new KeyValuePair<int, string>(supplierID, supplierName));
                    }

                    // Gán danh sách KeyValuePair vào ComboBox
                    comboBox2.DataSource = supplierList;
                    comboBox2.DisplayMember = "Value";
                    comboBox2.ValueMember = "Key";

                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng kết nối được đóng sau khi sử dụng
                dbManager.CloseConnection();
            }
        }

        private void LoadDepartmentIntoComboBox()
        {
            string query = "SELECT DepartmentID, DepartmentName FROM departments";
            DataTable dataTable = new DataTable();

            try
            {
                // Mở kết nối tới cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    DataRow zeroRow = dataTable.NewRow();
                    zeroRow["DepartmentID"] = 0;
                    zeroRow["DepartmentName"] = ""; // Bạn có thể gán một giá trị mặc định cho tên mục này
                    dataTable.Rows.InsertAt(zeroRow, 0);
                    // Gán dữ liệu vào comboBox1

                    comboBox8.DataSource = dataTable;
                    comboBox8.DisplayMember = "DepartmentName";
                    comboBox8.ValueMember = "DepartmentID";
                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng kết nối được đóng sau khi sử dụng
                dbManager.CloseConnection();
            }
        }
        private void LoadDepartmentIntoComboBox2()
        {
            string query = "SELECT DepartmentID, DepartmentName FROM departments";
            DataTable dataTable = new DataTable();

            try
            {
                // Mở kết nối tới cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);

                    // Tạo danh sách KeyValuePair để chứa các cặp giá trị DepartmentID và DepartmentName
                    List<KeyValuePair<int, string>> departmentList = new List<KeyValuePair<int, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int departmentID = Convert.ToInt32(row["DepartmentID"]);
                        string departmentName = row["DepartmentName"].ToString();
                        departmentList.Add(new KeyValuePair<int, string>(departmentID, departmentName));
                    }

                    // Gán danh sách KeyValuePair vào comboBox3
                    comboBox3.DataSource = departmentList;
                    comboBox3.DisplayMember = "Value";
                    comboBox3.ValueMember = "Key";

                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng kết nối được đóng sau khi sử dụng
                dbManager.CloseConnection();
            }
        }

        private void LoadEmployeesIntoComboBox()
        {
            string query = "SELECT EmployeeID, FirstName, LastName FROM employees";
            DataTable dataTable = new DataTable();

            try
            {
                // Mở kết nối tới cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    dataTable.Columns.Add("EmployeeInfo", typeof(string), "EmployeeID + ' - ' + LastName + ' ' + FirstName");

                    DataRow zeroRow = dataTable.NewRow();
                    zeroRow["EmployeeID"] = 0;
                    zeroRow["EmployeeInfo"] = ""; // Bạn có thể gán một giá trị mặc định cho tên mục này
                    dataTable.Rows.InsertAt(zeroRow, 0);
                    // Gán dữ liệu vào comboBox4

                    comboBox7.DataSource = dataTable;
                    comboBox7.DisplayMember = "EmployeeInfo"; // Tên của cột ảo
                    comboBox7.ValueMember = "EmployeeID";

                    // Tạo một cột ảo để hiển thị ID, LastName và FirstName cùng một dòng
                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng kết nối được đóng sau khi sử dụng
                dbManager.CloseConnection();
            }
        }
        private void LoadEmployeesIntoComboBox2()
        {
            string query = "SELECT EmployeeID, FirstName, LastName FROM employees";
            DataTable dataTable = new DataTable();

            try
            {
                // Mở kết nối tới cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);

                    // Tạo danh sách KeyValuePair để chứa các cặp giá trị EmployeeID và EmployeeInfo
                    List<KeyValuePair<int, string>> employeeList = new List<KeyValuePair<int, string>>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        int employeeID = Convert.ToInt32(row["EmployeeID"]);
                        string firstName = row["FirstName"].ToString();
                        string lastName = row["LastName"].ToString();
                        string employeeInfo = $"{employeeID} - {lastName} {firstName}";
                        employeeList.Add(new KeyValuePair<int, string>(employeeID, employeeInfo));
                    }

                    // Gán danh sách KeyValuePair vào comboBox4
                    comboBox4.DataSource = employeeList;
                    comboBox4.DisplayMember = "Value";
                    comboBox4.ValueMember = "Key";
                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                // Đảm bảo rằng kết nối được đóng sau khi sử dụng
                dbManager.CloseConnection();
            }
        }

        private void AddStaticDataToComboBox()
        {
            // Thêm dữ liệu tĩnh vào ComboBox
            comboBox6.Items.Add("");
            comboBox6.Items.Add("Đang sử dụng");
            comboBox6.Items.Add("Đang bảo trì");
            comboBox6.Items.Add("Cần bảo trì");
            comboBox6.Items.Add("Cần thanh lý");
            comboBox6.Items.Add("Đã thanh lý");
            comboBox5.Items.Add("Đang sử dụng");
            comboBox5.Items.Add("Đang bảo trì");
            comboBox5.Items.Add("Cần bảo trì");
            comboBox5.Items.Add("Cần thanh lý");
            comboBox5.Items.Add("Đã thanh lý");
            // Tiếp tục thêm các mục khác nếu cần thiết
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(selectedImagePath) && !string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(richTextBox1.Text) && !string.IsNullOrEmpty(comboBox5.Text))
                {
                    string assetName1 = textBox1.Text;
                    int assetTypeID1 = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
                    int supplierID1 = ((KeyValuePair<int, string>)comboBox2.SelectedItem).Key;
                    int departmentID1 = ((KeyValuePair<int, string>)comboBox3.SelectedItem).Key;
                    int employeeID1 = ((KeyValuePair<int, string>)comboBox4.SelectedItem).Key;
                    decimal value1 = decimal.Parse(textBox2.Text);
                    DateTime purchaseDate1 = dateTimePicker1.Value;
                    DateTime warrantyDate1 = dateTimePicker2.Value;
                    string status1 = comboBox5.SelectedItem.ToString();
                    string description1 = richTextBox1.Text;


                    if (dbManager.OpenConnection() )
                    {
                        // Lưu tên tập tin
                        string imageName = Path.GetFileName(selectedImagePath);


                        // Sao chép tập tin hình ảnh vào thư mục của dự án
                        string destinationDirectory = Path.Combine(Application.StartupPath, "Images");
                        string destinationPath = Path.Combine(destinationDirectory, imageName);

                        // Kiểm tra xem tệp đã tồn tại trong thư mục đích hay không
                        if (!File.Exists(destinationPath))
                        {
                            // Nếu tệp không tồn tại, thực hiện sao chép
                            File.Copy(selectedImagePath, destinationPath, true);
                        }

                        string tempImagePath = Path.Combine(Path.GetTempPath(), imageName);

                        // Lấy tên file hình ảnh

                        // Đường dẫn tạm thời để lưu trữ hình ảnh

                        // Sao chép tệp tin hình ảnh vào đường dẫn tạm thời
                        File.Copy(selectedImagePath, tempImagePath, true);

                        // Thêm tên tập tin hình ảnh và các thông tin khác vào cơ sở dữ liệu
                        string query = $"INSERT INTO fixedassets (AssetName, AssetTypeID, SupplierID, DepartmentID, EmployeeID, Value, PurchaseDate, Status, WarrantyDate, Image, Description) " +
                                       $"VALUES ('{assetName1}', {assetTypeID1}, {supplierID1}, {departmentID1}, {employeeID1}, {value1}, '{purchaseDate1:yyyy-MM-dd}', '{status1}', '{warrantyDate1:yyyy-MM-dd}', '{imageName}','{description1}')";
                        MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Dữ liệu đã được thêm thành công.");
                            // Xóa dữ liệu từ các ComboBox và TextBox
                            comboBox1.SelectedIndex = -1;
                            comboBox2.SelectedIndex = -1;
                            comboBox3.SelectedIndex = -1;
                            comboBox4.SelectedIndex = -1;
                            comboBox5.SelectedIndex = -1;
                            textBox1.Text = "";
                            textBox2.Text = "";
                            openFileDialog.FileName = null;
                            pictureBox1.Image = null;
                            richTextBox1.Text = null;
                            panel2.Visible = true;


                        }
                        else
                        {
                            MessageBox.Show("Không thể thêm dữ liệu.");
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
                    MessageBox.Show("Vui lòng nhập đủ thông tin.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Vui lòng nhập đúng giá trị sản phẩm" + ex.Message);
            }
            // Load lại dữ liệu vào DataGridView
            LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
            AddColumnsAndEventHandlers();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            currentPage = 1;

            searchKeyword = maskedTextBox1.Text;

            LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
            AddColumnsAndEventHandlers();


        }


        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem ID có hợp lệ không
                if (id > 0)
                {
                    // Kiểm tra xem selectedImagePath có giá trị không
                    if (!string.IsNullOrEmpty(selectedImagePath))
                    {
                        // Lấy dữ liệu từ các controls trên form
                        if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(richTextBox1.Text))
                        {
                            string assetName1 = textBox1.Text;
                            int assetTypeID1 = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
                            int supplierID1 = ((KeyValuePair<int, string>)comboBox2.SelectedItem).Key;
                            int departmentID1 = ((KeyValuePair<int, string>)comboBox3.SelectedItem).Key;
                            int employeeID1 = ((KeyValuePair<int, string>)comboBox4.SelectedItem).Key;
                            decimal value1 = decimal.Parse(textBox2.Text);
                            DateTime purchaseDate1 = dateTimePicker1.Value;
                            DateTime warrantyDate1 = dateTimePicker2.Value;
                            string status1 = comboBox5.Text;
                            string description1 = richTextBox1.Text;

                            // Thực hiện truy vấn cập nhật dữ liệu
                            if (dbManager.OpenConnection())
                            {
                                string imageName = Path.GetFileName(selectedImagePath);

                                // Sao chép tập tin hình ảnh vào thư mục của dự án
                                string destinationDirectory = Path.Combine(Application.StartupPath, "Images");
                                string destinationPath = Path.Combine(destinationDirectory, imageName);

                                // Kiểm tra xem tệp đã tồn tại trong thư mục đích hay không
                                if (!File.Exists(destinationPath))
                                {
                                    // Nếu tệp không tồn tại, thực hiện sao chép
                                    File.Copy(selectedImagePath, destinationPath, true);
                                }

                                string tempImagePath = Path.Combine(Path.GetTempPath(), imageName);

                                // Lấy tên file hình ảnh

                                // Đường dẫn tạm thời để lưu trữ hình ảnh

                                // Sao chép tệp tin hình ảnh vào đường dẫn tạm thời
                                File.Copy(selectedImagePath, tempImagePath, true);

                                // Thiết lập đường dẫn cho OpenFileDialog
                                openFileDialog.FileName = tempImagePath;

                                string query = $"UPDATE fixedassets SET AssetName = '{assetName1}', AssetTypeID = {assetTypeID1}, " +
                                           $"SupplierID = {supplierID1}, DepartmentID = {departmentID1}, EmployeeID = {employeeID1}, " +
                                           $"Value = {value1}, PurchaseDate = '{purchaseDate1:yyyy-MM-dd}', " +
                                           $"WarrantyDate = '{warrantyDate1:yyyy-MM-dd}', Status = '{status1}', Image = '{imageName}', Description = '{description1}' " +
                                           $"WHERE FixedAssetID = {id}";

                                MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                                int rowsAffected = command.ExecuteNonQuery();
                                if (rowsAffected > 0)
                                {
                                    // Sao chép hình ảnh vào thư mục của dự án


                                    MessageBox.Show("Dữ liệu đã được cập nhật thành công.");
                                    // Xóa dữ liệu từ các controls trên form
                                    comboBox1.SelectedIndex = -1;
                                    comboBox2.SelectedIndex = -1;
                                    comboBox3.SelectedIndex = -1;
                                    comboBox4.SelectedIndex = -1;
                                    comboBox5.SelectedIndex = -1;
                                    textBox1.Text = "";
                                    textBox2.Text = "";
                                    openFileDialog.FileName = null;
                                    pictureBox1.Image = null;
                                    richTextBox1.Text = null;
                                    panel2.Visible = true;
                                    id = 0;

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
                            MessageBox.Show("Vùi lòng nhập đủ thong tin");
                        }
                        }
                    else
                    {
                        MessageBox.Show("Vui lòng chọn hình ảnh trước khi cập nhật.");
                    }
                }
                else
                {
                    button1_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }

            // Load lại dữ liệu vào DataGridView
            LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
            AddColumnsAndEventHandlers();
        }


        // Sự kiện Click cho nút export
        private void button6_Click(object sender, EventArgs e)
        {
            string searchKeyword = maskedTextBox1.Text; // Giả sử giá trị searchKeyword được nhập từ textbox textBoxSearch
            ExportDataBeforePagination(searchKeyword, assetTypeID, departmentID, employeeID, status);
        }

        private void ExportDataBeforePagination(string searchKeyword, int assetTypeID, int departmentID, int employeeID, string status)
        {
            try
            {
                DataTable searchData = assetBusiness.GetAssetData(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
                if (searchData != null)
                {
                    ExportToExcel(searchData);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        public void ExportToExcel(DataTable dataTable)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // Đổ tiêu đề cột từ DataTable vào Excel
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dataTable.Columns[i].ColumnName;
                    }

                    // Đổ dữ liệu từ DataTable vào Excel
                    for (int row = 0; row < dataTable.Rows.Count; row++)
                    {
                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            var cellValue = dataTable.Rows[row][col];
                            if (cellValue != DBNull.Value)
                            {
                                // Kiểm tra kiểu dữ liệu của ô
                                if (dataTable.Columns[col].DataType == typeof(int) ||
                                    dataTable.Columns[col].DataType == typeof(decimal) ||
                                    dataTable.Columns[col].DataType == typeof(double) ||
                                    dataTable.Columns[col].DataType == typeof(float))
                                {
                                    // Chuyển đổi sang kiểu số nếu là kiểu số
                                    worksheet.Cell(row + 2, col + 1).Value = Convert.ToDouble(cellValue);
                                }
                                else
                                {
                                    // Giữ nguyên giá trị nếu không phải là kiểu số
                                    worksheet.Cell(row + 2, col + 1).Value = cellValue.ToString();
                                }
                            }
                        }
                    }

                    worksheet.Columns().AdjustToContents();
                    // Lưu workbook vào một tập tin Excel
                    using (var saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                        saveFileDialog.FileName = "Asset_list_Exported";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            workbook.SaveAs(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }




        private void button7_Click(object sender, EventArgs e)
        {
            panel1.Parent = panel2; // Đặt Parent của Panel là null
            panel1.Visible = true;
            panel1.BringToFront(); // Đảm bảo Panel được vẽ trên tất cả các control khác
            panel2.Visible = false; 
            panel4.Visible = false;

            //panel2.BringToFront(); // Đảm bảo Panel được vẽ trên tất cả các control khác
            ShowPanelInCenter(panel1, this);
        }
        private void ShowPanelInCenter(Panel panel, UserControl userControl)
        {
            // Xác định kích thước của UserControl và Panel
            int userControlWidth = userControl.Width;
            int userControlHeight = userControl.Height;
            int panelWidth = panel.Width;
            int panelHeight = panel.Height;
            // Tính toán vị trí của Panel để nó được đặt ở giữa UserControl
            int panelX = (userControlWidth - panelWidth) / 2;
            int panelY = (userControlHeight - panelHeight) / 2;
            // Đặt vị trí của Panel
            panel.Location = new Point(panelX, panelY);
            // Thêm Panel vào UserControl
            userControl.Controls.Add(panel);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.png;*.gif)|*.bmp;*.jpg;*.png;*.gif|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = openFileDialog.FileName;
                // Hiển thị hình ảnh trong PictureBox
                pictureBox1.ImageLocation = selectedImagePath;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
        private void UpdatePaginationControls()
        {
            label10.Text = currentPage.ToString();
            label11.Text = totalPages.ToString();
            button2.Enabled = currentPage > 1;
            button3.Enabled = currentPage < totalPages;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
                AddColumnsAndEventHandlers();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
                AddColumnsAndEventHandlers();
            }


        }

        
        private void button9_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            id = 0;
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            comboBox5.SelectedIndex = -1;
            textBox1.Text = "";
            textBox2.Text = "";
            openFileDialog.FileName = null;
            pictureBox1.Image = null;
            richTextBox1.Text = null;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            currentPage = 1;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
            comboBox9.SelectedIndex = 0;
            maskedTextBox1.Text = "";
            searchKeyword = "";
            assetTypeID = 0;
            departmentID = 0;
            employeeID = 0;
            status = null;
            FromDate = DateTime.MinValue;
            ToDate = DateTime.MaxValue;
            FromWarranty = DateTime.MinValue;
            ToWarranty = DateTime.MaxValue;
        LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
            AddColumnsAndEventHandlers();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
      
            assetTypeID = Convert.ToInt32(comboBox9.SelectedValue);
            departmentID = Convert.ToInt32(comboBox8.SelectedValue);
            employeeID = comboBox7.SelectedItem != null ? Convert.ToInt32(comboBox7.SelectedValue) : 0;
            status = comboBox6.Text;
            currentPage = 1;

            // Lấy giá trị AssetName từ TextBox
            searchKeyword = maskedTextBox1.Text;
            FromDate = dateTimePicker3.Value;
            ToDate = dateTimePicker4.Value;
            FromWarranty = dateTimePicker5.Value;
            ToWarranty = dateTimePicker6.Value;
            LoadDataIntoDataGridView(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
            AddColumnsAndEventHandlers();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            panel4.Visible = false;
            panel2.Visible = true;
            dataGridView2.DataSource = null; // Bỏ kết nối với dữ liệu
            dataGridView2.Rows.Clear(); // Xóa hết các dòng
            dataGridView2.Columns.Clear(); // Xóa hết các cột
        }

    
        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (assetID > 0)
            {
                try
                {

                    if (dbManager.OpenConnection())
                    {
                        string query = $"SELECT f.AssetName AS Name, a.AssetTypeName AS Type, s.SupplierName AS Supplier, d.DepartmentName AS Department, CONCAT(e.LastName, ' ', e.FirstName) AS Employee, f.Value, f.PurchaseDate AS Date, f.WarrantyDate AS Warranty, f.Status, f.Description, f.Image " +
                                       $"FROM fixedassets f " +
                                       $"JOIN assettypes a ON f.AssetTypeID = a.AssetTypeID " +
                                       $"JOIN departments d ON f.DepartmentID = d.DepartmentID " +
                                       $"JOIN suppliers s ON f.SupplierID = s.SupplierID " +
                                       $"JOIN employees e ON f.EmployeeID = e.EmployeeID " +
                                       $"WHERE f.FixedAssetID = {assetID}";

                        MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Lấy dữ liệu từ cơ sở dữ liệu
                            id = assetID;
                            string assetName1 = reader["Name"].ToString();
                            string assetType1 = reader["Type"].ToString();
                            string supplier1 = reader["Supplier"].ToString();
                            string department1 = reader["Department"].ToString();
                            string employee1 = reader["Employee"].ToString();
                            decimal value1 = Convert.ToDecimal(reader["Value"]);
                            DateTime purchaseDate1 = Convert.ToDateTime(reader["Date"]);
                            DateTime warrantyDate1 = Convert.ToDateTime(reader["Warranty"]);
                            string status1 = reader["Status"].ToString();
                            string description1 = reader["Description"].ToString();
                            string imageName1 = reader["Image"].ToString();

                            // Hiển thị dữ liệu trên các controls tương ứng
                            textBox1.Text = assetName1;

                            // Tìm và gán giá trị cho ComboBox Type
                            foreach (KeyValuePair<int, string> item in comboBox1.Items)
                            {
                                if (item.Value == assetType1)
                                {
                                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(item);
                                    break;
                                }
                            }
                            foreach (KeyValuePair<int, string> item in comboBox2.Items)
                            {
                                if (item.Value == supplier1)
                                {
                                    comboBox2.SelectedIndex = comboBox2.Items.IndexOf(item);
                                    break;
                                }
                            }

                            foreach (KeyValuePair<int, string> item in comboBox3.Items)
                            {
                                if (item.Value == department1)
                                {
                                    comboBox3.SelectedIndex = comboBox3.Items.IndexOf(item);
                                    break;
                                }
                            }
                            foreach (KeyValuePair<int, string> item in comboBox4.Items)
                            {
                                // Trích xuất phần lastName và firstName từ item.Value
                                string[] parts = item.Value.Split('-');
                                string fullName = parts[1].Trim(); // Loại bỏ dấu cách thừa

                                // So sánh fullName với employee1
                                if (fullName == employee1)
                                {
                                    comboBox4.SelectedItem = item;
                                    break;
                                }
                            }
                            textBox2.Text = value1.ToString();
                            dateTimePicker1.Value = purchaseDate1;
                            dateTimePicker2.Value = warrantyDate1;
                            comboBox5.SelectedItem = status1;
                            richTextBox1.Text = description1;

                            // Load hình ảnh từ tên tập tin lấy được từ cơ sở dữ liệu
                            string imagePath = Path.Combine(Application.StartupPath, "Images", imageName1);
                            pictureBox1.ImageLocation = imagePath;
                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                            // Gán giá trị cho selectedImagePath
                            selectedImagePath = imagePath;

                            // Đảm bảo rằng OpenFileDialog chứa đúng đường dẫn của hình ảnh
                            if (File.Exists(imagePath) && id > 0)
                            {
                                openFileDialog.FileName = imagePath;
                                button7_Click(sender, e);
                            }
                            else
                            {
                                openFileDialog.FileName = string.Empty;
                                button7_Click(sender, e);
                            }
                        }

                        reader.Close();
                        dbManager.CloseConnection();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //button13_Click(sender, e);

        }

        private void button13_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = assetBusiness.GetAllTransfer(id);
            dataGridView2.Columns[0].Width = 30;
            dataGridView2.Columns[1].Width = 150;
            dataGridView2.Columns[2].Width = 80;
            dataGridView2.Columns[3].Width = 185;
            dataGridView2.Columns[4].Width = 120;
            dataGridView2.Columns[5].Width = 120;
            dataGridView2.Columns[6].Width = 120;
            dataGridView2.Columns[7].Width = 120;
            dataGridView2.Columns[8].Width = 80;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = assetBusiness.GetAllRepair(id);
            dataGridView2.Columns[0].Width = 30;
            dataGridView2.Columns[1].Width = 150;
            dataGridView2.Columns[2].Width = 80;
            dataGridView2.Columns[3].Width = 185;
            dataGridView2.Columns[4].Width = 70;
            dataGridView2.Columns[5].Width = 150;
            dataGridView2.Columns[6].Width = 140;
            dataGridView2.Columns[7].Width = 120;
            dataGridView2.Columns[8].Width = 80;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = assetBusiness.GetAllDisposal(id);
            dataGridView2.Columns[0].Width = 30;
            dataGridView2.Columns[1].Width = 220;
            dataGridView2.Columns[2].Width = 80;
            dataGridView2.Columns[3].Width = 255;
            dataGridView2.Columns[4].Width = 120;
            dataGridView2.Columns[5].Width = 150;
            dataGridView2.Columns[6].Width = 150;
        }
    }
    
}


