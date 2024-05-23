using AsserManagement.BUS;
using AsserManagement.DTO;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Paragraph = iTextSharp.text.Paragraph;

namespace AsserManagement
{
    public partial class GUIRepair : UserControl
    {
        private BUSRepair bUSRepair;
        string id;
        private DateTime FromDate;
        private DateTime ToDate;
        string searchKeyword;
        int assetTypeID;
        int departmentID;
        string status;
        public GUIRepair()
        {
            InitializeComponent();
            bUSRepair = new BUSRepair();

            LoadRepair(FromDate, ToDate);
            LoadDataIntoComboBox1();
            LoadDataIntoComboBox4();
            AddStaticDataToComboBox();
            LoadDataIntoComboBox9();
            LoadDataIntoComboBox5(searchKeyword, assetTypeID, departmentID, status);

        }
        private void LoadRepair(DateTime FromDate, DateTime ToDate)
        {
            dataGridView1.DataSource = bUSRepair.GetAllRepair(FromDate, ToDate);
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[1].Width = 150;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[3].Width = 185;
            dataGridView1.Columns[4].Width = 70;
            dataGridView1.Columns[5].Width = 150;
            dataGridView1.Columns[6].Width = 140;
            dataGridView1.Columns[7].Width = 120;
            dataGridView1.Columns[8].Width = 80;

        }
        private void LoadDataIntoComboBox5(string searchKeyword, int assetTypeID, int departmentID, string status)
        {

            List<KeyValuePair<int, string>> keyValuePairs = bUSRepair.GetAssetKeyValuePairList(searchKeyword, assetTypeID, departmentID, status);

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                comboBox5.DataSource = new BindingSource(keyValuePairs, null);
                comboBox5.DisplayMember = "Value";
                comboBox5.ValueMember = "Key";
            }
        }
        private void LoadDataIntoComboBox1()
        {
            List<KeyValuePair<int, string>> keyValuePairs = bUSRepair.GetDepartmentKeyValuePairList();

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                comboBox1.DataSource = new BindingSource(keyValuePairs, null);
                comboBox1.DisplayMember = "Value";
                comboBox1.ValueMember = "Key";
            }
            List<KeyValuePair<int, string>> keyValuePairs2 = bUSRepair.GetDepartmentKeyValuePairList();
            keyValuePairs2.Insert(0, new KeyValuePair<int, string>(-1, "")); // Sử dụng giá trị key và value tùy chọn cho mục rỗng

            if (keyValuePairs2 != null && keyValuePairs2.Count > 0)
            {
                comboBox8.DataSource = new BindingSource(keyValuePairs2, null);
                comboBox8.DisplayMember = "Value";
                comboBox8.ValueMember = "Key";
            }
        }
        private void LoadDataIntoComboBox4()
        {
            List<KeyValuePair<int, string>> keyValuePairs = bUSRepair.GetEmployeeKeyValuePairList();

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                comboBox4.DataSource = new BindingSource(keyValuePairs, null);
                comboBox4.DisplayMember = "Value";
                comboBox4.ValueMember = "Key";
            }
        }
        private void LoadDataIntoComboBox9()
        {
            List<KeyValuePair<int, string>> keyValuePairs = bUSRepair.GetTypeKeyValuePairList();
            keyValuePairs.Insert(0, new KeyValuePair<int, string>(-1, "")); // Sử dụng giá trị key và value tùy chọn cho mục rỗng

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                comboBox9.DataSource = new BindingSource(keyValuePairs, null);
                comboBox9.DisplayMember = "Value";
                comboBox9.ValueMember = "Key";
            }
        }
        private void AddStaticDataToComboBox()
        {
            // Thêm dữ liệu tĩnh vào ComboBox
            comboBox2.Items.Add("Tiến hành bảo trì");
            comboBox2.Items.Add("Bảo trì hoàn thành");
            comboBox2.Items.Add("Không thể bảo trì");
            comboBox2.Text = "";

            comboBox6.Text = "Cần bảo trì";
            status = comboBox6.Text;

            comboBox6.Items.Add("");
            comboBox6.Items.Add("Cần bảo trì");
            comboBox6.Items.Add("Đang sử dụng");
            comboBox6.Items.Add("Đang bảo trì");
            comboBox6.Items.Add("Cần thanh lý");
            comboBox6.Items.Add("Đã thanh lý");
            // Tiếp tục thêm các mục khác nếu cần thiết
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int fixedAssetID = (int)comboBox5.SelectedValue;
            DateTime repairDate = dateTimePicker3.Value;
            DateTime nextRepairDate = dateTimePicker4.Value;
            int departmentID = (int)comboBox1.SelectedValue;
            int employeeID = (int)comboBox4.SelectedValue;

            decimal repairCost;
            if (!decimal.TryParse(maskedTextBox3.Text, out repairCost))
            {
                MessageBox.Show("Giá trị chi phí sửa chữa không hợp lệ.");
                return; // Dừng lại nếu maskedTextBox3.Text không chứa số hợp lệ
            }

            string description = maskedTextBox1.Text;
            string status = comboBox2.Text;

            if (repairCost == 0 || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một biến không hợp lệ
            }

            bool result = bUSRepair.AddRepair(fixedAssetID, repairDate, description, repairCost, departmentID, employeeID, status, nextRepairDate);
            if (result)
            {
                MessageBox.Show("Created successfully.");
                ClearFields1();
            }
            else
            {
                MessageBox.Show("Failed to add asset type.");
            }
            LoadRepair(FromDate, ToDate);
        }

        private void ClearFields1()
        {
            maskedTextBox1.Text = "";
            maskedTextBox2.Text = "";
            maskedTextBox3.Text = "";
            dateTimePicker3.Text = "";
            dateTimePicker4.Text = "";
            comboBox1.Text = string.Empty;
            comboBox4.Text = string.Empty;
            comboBox5.Text = string.Empty;

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn một hàng không
            if (e.RowIndex >= 0)
            {
                comboBox5.Enabled = false;

                // Lấy giá trị của cột ID trong hàng được chọn
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                id = selectedRow.Cells["ID"].Value.ToString(); // Thay "IDColumnName" bằng tên cột ID trong DataGridView


                // Lấy thông tin AssetType từ BUS
                RepairsAndMaintenance repairsAndMaintenance = bUSRepair.GetRepairsAndMaintenanceById(id);

                if (repairsAndMaintenance != null)
                {
                    foreach (KeyValuePair<int, string> item in comboBox5.Items)
                    {
                        if (item.Key == repairsAndMaintenance.FixedAssetID)
                        {
                            comboBox5.SelectedIndex = comboBox5.Items.IndexOf(item);
                            break;
                        }
                    }
                    foreach (KeyValuePair<int, string> item in comboBox1.Items)
                    {
                        if (item.Key == repairsAndMaintenance.DepartmentID)
                        {
                            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(item);
                            break;
                        }
                    }
                    foreach (KeyValuePair<int, string> item in comboBox4.Items)
                    {
                        if (item.Key == repairsAndMaintenance.EmployeeID)
                        {
                            comboBox4.SelectedIndex = comboBox4.Items.IndexOf(item);
                            break;
                        }
                    }
                    // Hiển thị thông tin trên giao diện
                    maskedTextBox1.Text = repairsAndMaintenance.Description;
                    maskedTextBox3.Text = repairsAndMaintenance.RepairCost.ToString();
                    dateTimePicker3.Value = repairsAndMaintenance.RepairDate;
                    dateTimePicker4.Value = repairsAndMaintenance.NextRepairDate;
                    comboBox2.SelectedItem = repairsAndMaintenance.Status;
                    button4.Visible = true;

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
            id = string.Empty;
            maskedTextBox3.Text = "";
            maskedTextBox1.Text = "";
            LoadDataIntoComboBox5(searchKeyword, assetTypeID, departmentID, status);
            LoadDataIntoComboBox1();
            LoadDataIntoComboBox4();
            AddStaticDataToComboBox();
            comboBox5.Enabled = true;
            dateTimePicker3.Value = DateTime.Today;
            dateTimePicker4.Value = DateTime.Today;
            button4.Visible = false;
            button8.Visible = false;
            button9.Visible = false;
            button2.Visible = true;
            button12.Visible = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {

            int fixedAssetID = (int)comboBox5.SelectedValue;
            DateTime repairDate = dateTimePicker3.Value;
            DateTime nextRepairDate = dateTimePicker4.Value;
            int departmentID = (int)comboBox1.SelectedValue;
            int employeeID = (int)comboBox4.SelectedValue;

            decimal repairCost;
            if (!decimal.TryParse(maskedTextBox3.Text, out repairCost))
            {
                MessageBox.Show("Giá trị chi phí sửa chữa không hợp lệ.");
                return; // Dừng lại nếu maskedTextBox3.Text không chứa số hợp lệ
            }

            string description = maskedTextBox1.Text;
            string status = comboBox2.Text;

            if (repairCost == 0 || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một biến không hợp lệ
            }

            bool result = bUSRepair.UpdateRepairsAndMaintenance(id, fixedAssetID, repairDate, description, repairCost, departmentID, employeeID, status, nextRepairDate);
            if (result)
            {
                MessageBox.Show("Cập nhật thành công.");
                button12_Click(sender, e);
                LoadRepair(FromDate, ToDate);
                // Gọi hàm để tải lại dữ liệu hoặc thực hiện hành động khác sau khi cập nhật thành công.
            }
            else
            {
                MessageBox.Show("Cập nhật thất bại.");
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
                if (bUSRepair.DeleteRepairsAndMaintenance(id))
                {
                    MessageBox.Show("Xóa thông tin thành công.");
                    LoadRepair(FromDate, ToDate);
                    button12_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Không thể xóa bản ghi.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu DataGridView có dữ liệu
            DataTable dataTable = bUSRepair.GetAllRepair(FromDate, ToDate);

            // Gọi phương thức từ lớp BUS để xuất dữ liệu sang Excel
            bUSRepair.ExportToExcel(dataTable);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ToDate = dateTimePicker1.Value;
            FromDate = dateTimePicker2.Value;
            LoadRepair(FromDate, ToDate);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            searchKeyword = maskedTextBox2.Text;
            if (comboBox8.SelectedValue != null)
            { 
                    departmentID = (int)comboBox8.SelectedValue;
            }
            if (comboBox9.SelectedValue != null)
            {
                assetTypeID = (int)comboBox9.SelectedValue;
            }
            status = comboBox6.Text;
            LoadDataIntoComboBox5(searchKeyword, assetTypeID, departmentID, status);


        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Tạo một tài liệu mới
            Document document = new Document();

            // Tạo một save file dialog
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog1.FileName = "Repair_Bill";

            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string outputPath = Path.GetDirectoryName(saveFileDialog1.FileName);
                string outputFilePath = saveFileDialog1.FileName;

                try
                {
                    // Tạo writer với UTF-8 encoding
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputFilePath, FileMode.Create));

                    // Sử dụng UTF-8 encoding
                    writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
                    writer.CompressionLevel = PdfStream.NO_COMPRESSION;

                    // Mở tài liệu
                    document.Open();

                    // Sử dụng font Unicode cho tiếng Việt
                    BaseFont bf1 = BaseFont.CreateFont(@"C:\Windows\Fonts\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    Font font = new Font(bf1, 12);
                    BaseFont bf2 = BaseFont.CreateFont(@"C:\Windows\Fonts\arialbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                    Font titlefont = new Font(bf2, 12);
                    BaseFont bf3 = BaseFont.CreateFont(@"C:\Windows\Fonts\arialbd.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                    Font titlefont1 = new Font(bf3, 36);


                    // Thêm dữ liệu từ các điều khiển vào tài liệu PDF
                    Paragraph paragraph = new Paragraph();
                   
                    Paragraph title = new Paragraph("Phiếu Bảo Trì", titlefont1);
                    paragraph.Add("\n");
                    paragraph.Add("\n");

                  
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    paragraph.Add(new Chunk("Mã bảo trì : ", titlefont));
                    paragraph.Add(new Chunk("RM0" + id, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Tên tài sản bảo trì : ", titlefont));
                    paragraph.Add(new Chunk(comboBox5.Text, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Mã tài sản bảo trì : ", titlefont));
                    paragraph.Add(new Chunk("AS0"+ (int)comboBox5.SelectedValue, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Giá bảo trì : ", titlefont));
                    paragraph.Add(new Chunk("$"+maskedTextBox3.Text, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Phòng ban phụ trách : ", titlefont));
                    paragraph.Add(new Chunk(comboBox1.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Nhân viên phụ trách : ", titlefont));
                    paragraph.Add(new Chunk(comboBox4.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Chi tiết bảo trì : ", titlefont));
                    paragraph.Add(new Chunk(maskedTextBox1.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Ngày bảo trì : ", titlefont));
                    paragraph.Add(new Chunk(dateTimePicker3.Value.ToShortDateString(), font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Hạn bảo trì tiếp theo : ", titlefont));
                    paragraph.Add(new Chunk(dateTimePicker4.Value.ToShortDateString(), font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Tình trạng bảo trì : ", titlefont));
                    paragraph.Add(new Chunk(comboBox2.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("                                    _________________________________________", titlefont));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n"); paragraph.Add("\n");

                    paragraph.Add(new Chunk("                            Đơn vị bảo trì                                                       Đơn vị phụ trách", font));

                    document.Add(paragraph);

                    // Đóng tài liệu
                    document.Close();

                    MessageBox.Show("Xuất thành công PDF: " + outputFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            maskedTextBox2.Text="";
            comboBox8.SelectedValue="";
            comboBox9.SelectedValue="";
            comboBox6.Text="";
            searchKeyword = null;
            departmentID = 0;
            assetTypeID = 0;
            status = null;
            LoadDataIntoComboBox5(searchKeyword, assetTypeID, departmentID, status);
        }
    }
}
