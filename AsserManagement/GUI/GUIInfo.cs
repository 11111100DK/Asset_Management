using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
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
    public partial class GUIInfo : UserControl
    {
        private BUSInfo bUSInfo;
        string id1;
        string id2;
        string id3;

        public GUIInfo()
        {
            InitializeComponent();
            bUSInfo = new BUSInfo();
            LoadAssetTypes();
            LoadDepartments();
            LoadEmployees();
            LoadDepartmentIntoComboBox();
            

        }

        private void LoadAssetTypes()
        {
            dataGridView1.DataSource = bUSInfo.GetAllAssetTypes();
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].Width = 200;
        }
        private void LoadDepartments()
        {
            dataGridView2.DataSource = bUSInfo.GetAllDepartments();
            dataGridView2.Columns[0].Width = 30;
            dataGridView2.Columns[1].Width = 100;
            dataGridView2.Columns[2].Width = 300;
        }
        private void LoadEmployees()
        {
            dataGridView3.DataSource = bUSInfo.GetAllEmployees();
            dataGridView3.Columns[0].Width = 30;
            dataGridView3.Columns[1].Width = 140;
            dataGridView3.Columns[2].Width = 142;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = maskedTextBox2.Text;
            string description = richTextBox2.Text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một TextBox rỗng
            }
            bool result = bUSInfo.AddAssetType(name, description);
            if (result)
            {
                MessageBox.Show("Asset type added successfully.");
                ClearFields1();
            }
            else
            {
                MessageBox.Show("Failed to add asset type.");
            }
            LoadAssetTypes();

        }
        private void ClearFields1()
        {
            maskedTextBox2.Text = "";
            richTextBox2.Text = "";
            
        }
        private void ClearFields2()
        {
            
            maskedTextBox1.Text = "";
            richTextBox1.Text = "";
        }
        private void ClearFields3()
        {
            maskedTextBox3.Text = "";
            maskedTextBox4.Text = "";
            comboBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = maskedTextBox1.Text;
            string description = richTextBox1.Text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một TextBox rỗng
            }
            bool result = bUSInfo.AddDepartment(name, description);
            if (result)
            {
                MessageBox.Show("Department added successfully.");
                ClearFields2();
            }
            else
            {
                MessageBox.Show("Failed to add department.");
            }
            LoadDepartments();
        }
        private void LoadDepartmentIntoComboBox()
        {
            List<KeyValuePair<int, string>> departmentList = bUSInfo.GetDepartmentList();

            // Gán danh sách KeyValuePair vào comboBox3
            comboBox1.DataSource = departmentList;
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = maskedTextBox3.Text;
            int idd = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
            string position = maskedTextBox4.Text;
            string last = "";
            string first = "";
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(position) || idd <= 0)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một TextBox rỗng
            }
            string[] parts = name.Split(' '); // Chia chuỗi dựa trên khoảng trắng

            if (parts.Length >= 2)
            {
                last = parts[0]; // Lấy phần đầu tiên
                first = string.Join(" ", parts.Skip(1)); // Kết hợp các phần tử từ chỉ mục thứ 1 đến cuối

                
            }
            else
            {
                last = name;
            }
            bool result = bUSInfo.AddEmployee(last, first, position, idd);
            if (result)
            {
                MessageBox.Show("Employee added successfully.");
                ClearFields3();
            }
            else
            {
                MessageBox.Show("Failed to add employee.");
            }
            LoadEmployees();
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
                AssetType assetType = bUSInfo.GetAssetTypeById(id1);

                if (assetType != null)
                {
                    // Hiển thị thông tin trên giao diện
                    maskedTextBox2.Text = assetType.Name;
                    richTextBox2.Text = assetType.Description;
                    button4.Visible = true;
                    button5.Visible = true;
                    button3.Visible = false;
                    button10.Visible = true;

                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin cho ID đã chọn.");
                }
            }
        
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn một hàng không
            if (e.RowIndex >= 0)
            {
                // Lấy giá trị của cột ID trong hàng được chọn
                DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];
                id2 = selectedRow.Cells["ID"].Value.ToString(); // Thay "IDColumnName" bằng tên cột ID trong DataGridView


                // Lấy thông tin AssetType từ BUS
                Department department = bUSInfo.GetDepartmentById(id2);

                if (department != null)
                {
                    // Hiển thị thông tin trên giao diện
                    maskedTextBox1.Text = department.Name;
                    richTextBox1.Text = department.Description;
                    button6.Visible = true;
                    button7.Visible = true;
                    button1.Visible = false;
                    button11.Visible = true;

                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin cho ID đã chọn.");
                }
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn một hàng không
            if (e.RowIndex >= 0)
            {
                // Lấy giá trị của cột ID trong hàng được chọn
                DataGridViewRow selectedRow = dataGridView3.Rows[e.RowIndex];
                id3 = selectedRow.Cells["ID"].Value.ToString(); // Thay "IDColumnName" bằng tên cột ID trong DataGridView


                // Lấy thông tin AssetType từ BUS
                Employee employee = bUSInfo.GetEmployeeById(id3);

                if (employee != null)
                {
                    // Hiển thị thông tin trên giao diện
                    maskedTextBox3.Text = employee.LastName + " " + employee.FirstName;
                    maskedTextBox4.Text = employee.Position;
                    foreach (KeyValuePair<int, string> item in comboBox1.Items)
                    {
                        if (item.Key == employee.DepartmentID)
                        {
                            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(item);
                            break;
                        }
                    }
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

        private void button10_Click(object sender, EventArgs e)
        {
            id1 = string.Empty;
            maskedTextBox2.Text = "";
            richTextBox2.Text = "";
            button4.Visible = false;
            button5.Visible = false;
            button3.Visible = true;
            button10.Visible = false;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            id2 = string.Empty;
            maskedTextBox1.Text = "";
            richTextBox1.Text = "";
            button6.Visible = false;
            button7.Visible = false;
            button1.Visible = true;
            button11.Visible = false;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            id3 = string.Empty;
            maskedTextBox3.Text = "";
            maskedTextBox4.Text = "";
            comboBox1.Text = "";
            button8.Visible = false;
            button9.Visible = false;
            button2.Visible = true;
            button12.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string newName = maskedTextBox2.Text;
            string newDescription = richTextBox2.Text;

            bool success = bUSInfo.UpdateAssetType(id1, newName, newDescription);

            if (success)
            {
                MessageBox.Show("Cập nhật thông tin thành công.");
                LoadAssetTypes();
                button10_Click(sender, e);

            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin.");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string newName = maskedTextBox1.Text;
            string newDescription = richTextBox1.Text;

            bool success = bUSInfo.UpdateDepartment(id2, newName, newDescription);

            if (success)
            {
                MessageBox.Show("Cập nhật thông tin thành công.");
                LoadDepartments();
                button11_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin.");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string name = maskedTextBox3.Text;
            string position = maskedTextBox4.Text;
            int idd = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;
            string last = "";
            string first = "";
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(position) || idd <= 0)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một TextBox rỗng
            }
            string[] parts = name.Split(' '); // Chia chuỗi dựa trên khoảng trắng

            if (parts.Length >= 2)
            {
                last = parts[0]; // Lấy phần đầu tiên
                first = string.Join(" ", parts.Skip(1)); // Kết hợp các phần tử từ chỉ mục thứ 1 đến cuối
            }
            else
            {
                last = name;
            }
            bool success = bUSInfo.UpdateEmployee(id3, last, first, position, idd);

            if (success)
            {
                MessageBox.Show("Cập nhật thông tin thành công.");
                LoadEmployees();
                button12_Click(sender, e);
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi cập nhật thông tin.");
            }


            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        
            // Kiểm tra kết quả của hộp thoại
            if (result == DialogResult.Yes)
            {
            // Xóa bản ghi từ ID
                if (bUSInfo.DeleteAssetType(id1))
                {
                    MessageBox.Show("Xóa thông tin thành công.");
                    LoadAssetTypes();
                    button10_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Không thể xóa bản ghi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            // Kiểm tra kết quả của hộp thoại
            if (result == DialogResult.Yes)
            {
                // Xóa bản ghi từ ID
                if (bUSInfo.DeleteDepartment(id2))
                {
                    MessageBox.Show("Xóa thông tin thành công.");
                    LoadDepartments();
                    button11_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Không thể xóa bản ghi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                if (bUSInfo.DeleteEmployee(id3))
                {
                    MessageBox.Show("Xóa thông tin thành công.");
                    LoadEmployees();
                    button12_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Không thể xóa bản ghi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
