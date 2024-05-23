using AsserManagement.BUS;
using AsserManagement.DTO;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
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
using Font = iTextSharp.text.Font;

namespace AsserManagement
{
    public partial class GUITransfer : UserControl
    {
        private BUSTransfer bUSTransfer;
        private DateTime FromDate;
        private DateTime ToDate;
        string searchKeyword;
        string id;
        int assetTypeID;
        int departmentID;
        string status;
        public GUITransfer()
        {
            InitializeComponent();
            bUSTransfer = new BUSTransfer();
            LoadTransfer(FromDate, ToDate);
            LoadDataIntoComboBox48();
            LoadDataIntoComboBox3();
            LoadDataIntoComboBox9();
            LoadDataIntoComboBox5(searchKeyword, assetTypeID, departmentID, status);
            AddStaticDataToComboBox();
        }
        private void LoadTransfer(DateTime FromDate, DateTime ToDate)
        {
            dataGridView1.DataSource = bUSTransfer.GetAllTransfer(FromDate, ToDate);
            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[1].Width = 150;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[3].Width = 185;
            dataGridView1.Columns[4].Width = 120;
            dataGridView1.Columns[5].Width = 120;
            dataGridView1.Columns[6].Width = 120;
            dataGridView1.Columns[7].Width = 120;
            dataGridView1.Columns[8].Width = 80;

        }
        private void LoadDataIntoComboBox5(string searchKeyword, int assetTypeID, int departmentID, string status)
        {

            List<KeyValuePair<int, string>> keyValuePairs = bUSTransfer.GetAssetKeyValuePairList(searchKeyword, assetTypeID, departmentID, status);

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                comboBox5.DataSource = new BindingSource(keyValuePairs, null);
                comboBox5.DisplayMember = "Value";
                comboBox5.ValueMember = "Key";
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ToDate = dateTimePicker1.Value;
            FromDate = dateTimePicker2.Value;
            LoadTransfer(FromDate, ToDate);
        }
        private void LoadDataIntoComboBox48()
        {
            List<KeyValuePair<int, string>> keyValuePairs = bUSTransfer.GetDepartmentKeyValuePairList();

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                comboBox4.DataSource = new BindingSource(keyValuePairs, null);
                comboBox4.DisplayMember = "Value";
                comboBox4.ValueMember = "Key";
            }
            List<KeyValuePair<int, string>> keyValuePairs2 = bUSTransfer.GetDepartmentKeyValuePairList();
            keyValuePairs2.Insert(0, new KeyValuePair<int, string>(0, "")); // Sử dụng giá trị key và value tùy chọn cho mục rỗng

            if (keyValuePairs2 != null && keyValuePairs2.Count > 0)
            {
                comboBox8.DataSource = new BindingSource(keyValuePairs2, null);
                comboBox8.DisplayMember = "Value";
                comboBox8.ValueMember = "Key";
            }
        }
        private void LoadDataIntoComboBox3()
        {
            List<KeyValuePair<int, string>> keyValuePairs = bUSTransfer.GetEmployeeKeyValuePairList();

            if (keyValuePairs != null && keyValuePairs.Count > 0)
            {
                comboBox3.DataSource = new BindingSource(keyValuePairs, null);
                comboBox3.DisplayMember = "Value";
                comboBox3.ValueMember = "Key";
            }
        }
        private void LoadDataIntoComboBox9()
        {
            List<KeyValuePair<int, string>> keyValuePairs = bUSTransfer.GetTypeKeyValuePairList();
            keyValuePairs.Insert(0, new KeyValuePair<int, string>(0, "")); // Sử dụng giá trị key và value tùy chọn cho mục rỗng

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

            comboBox6.Items.Add("Cần bảo trì");
            comboBox6.Items.Add("Đang sử dụng");
            comboBox6.Items.Add("Đang bảo trì");
            comboBox6.Items.Add("Cần thanh lý");
            comboBox6.Items.Add("Đã thanh lý");
            // Tiếp tục thêm các mục khác nếu cần thiết
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

        private void button10_Click(object sender, EventArgs e)
        {
            maskedTextBox2.Text = "";
            comboBox8.SelectedValue = "";
            comboBox9.SelectedValue = "";
            comboBox6.Text = "";
            searchKeyword = null;
            departmentID = 0;
            assetTypeID = 0;
            status = null;
            LoadDataIntoComboBox5(searchKeyword, assetTypeID, departmentID, status);

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedItem != null)
            {
                int fixedAssetID = ((KeyValuePair<int, string>)comboBox5.SelectedItem).Key;

                // Gọi các phương thức từ BUSRepair để lấy thông tin
                int departmentID = bUSTransfer.GetDepartmentIDByFixedAssetID(fixedAssetID);
                int employeeID = bUSTransfer.GetEmployeeIDByFixedAssetID(fixedAssetID);
                string departmentName = bUSTransfer.GetDepartmentNameByID(departmentID);
                string employeeName = bUSTransfer.GetEmployeeNameByID(employeeID);

                // Hiển thị thông tin lên các TextBox
                comboBox1.Text = departmentName;
                comboBox2.Text = employeeName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ giao diện người dùng
            int fixedAssetID = ((KeyValuePair<int, string>)comboBox5.SelectedItem).Key;
            DateTime transferDate = dateTimePicker3.Value;
            int fromDepartmentID = bUSTransfer.GetDepartmentIDByFixedAssetID(fixedAssetID);
            int fromEmployeeID = bUSTransfer.GetEmployeeIDByFixedAssetID(fixedAssetID);
            string transferReason = maskedTextBox1.Text;
            string notes = maskedTextBox3.Text;
            int toDepartmentID = ((KeyValuePair<int, string>)comboBox4.SelectedItem).Key;
            int toEmployeeID = ((KeyValuePair<int, string>)comboBox3.SelectedItem).Key;
            if (string.IsNullOrEmpty(transferReason) || string.IsNullOrEmpty(notes))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            // Gọi phương thức từ BUS để thêm bản ghi
            bool result = bUSTransfer.AddTransfer(fixedAssetID, transferDate, fromDepartmentID, toDepartmentID, transferReason, notes, fromEmployeeID, toEmployeeID);

            // Hiển thị thông báo kết quả
            if (result)
            {
                MessageBox.Show("Bản ghi đã được thêm thành công.");
                // Gọi phương thức để làm mới giao diện người dùng hoặc làm sạch các trường dữ liệu
            }
            else
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm bản ghi.");
            }
            LoadTransfer(FromDate, ToDate);

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

                // Lấy thông tin HistoryTransferAsset từ BUS
                HistoryTransferAsset historyTransferAsset = bUSTransfer.GetHistoryTransferAssetById(id);

                if (historyTransferAsset != null)
                {
                    // Gọi các phương thức để tải dữ liệu vào các ComboBox
                    LoadDataIntoComboBox48();
                    LoadDataIntoComboBox3();
                    LoadDataIntoComboBox9();

                    // Tìm kiếm và chọn giá trị tương ứng trong các ComboBox
                    foreach (KeyValuePair<int, string> item in comboBox5.Items)
                    {
                        if (item.Key == historyTransferAsset.FixedAssetID)
                        {
                            comboBox5.SelectedIndex = comboBox5.Items.IndexOf(item);
                            break;
                        }
                    }
                    foreach (KeyValuePair<int, string> item in comboBox4.Items)
                    {
                        if (item.Key == historyTransferAsset.ToDepartmentID)
                        {
                            comboBox4.SelectedIndex = comboBox4.Items.IndexOf(item);
                            break;
                        }
                    }
                    foreach (KeyValuePair<int, string> item in comboBox3.Items)
                    {
                        if (item.Key == historyTransferAsset.ToEmployeeID)
                        {
                            comboBox3.SelectedIndex = comboBox3.Items.IndexOf(item);
                            break;
                        }
                    }
                    string departmentName = bUSTransfer.GetDepartmentNameByID(historyTransferAsset.FromDepartmentID);
                    string employeeName = bUSTransfer.GetEmployeeNameByID(historyTransferAsset.FromEmployeeID);

                    // Hiển thị thông tin lên các TextBox
                    comboBox1.Text = departmentName;
                    comboBox2.Text = employeeName;

                    // Hiển thị thông tin trên giao diện
                    maskedTextBox1.Text = historyTransferAsset.TransferReason;
                    maskedTextBox3.Text = historyTransferAsset.Notes;
                    dateTimePicker3.Value = historyTransferAsset.TransferDate;

                   

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

        private void button9_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ giao diện người dùng
            int fixedAssetID = ((KeyValuePair<int, string>)comboBox5.SelectedItem).Key;
            DateTime transferDate = dateTimePicker3.Value;
            int fromDepartmentID = bUSTransfer.GetDepartmentIDByFixedAssetID(fixedAssetID);
            int fromEmployeeID = bUSTransfer.GetEmployeeIDByFixedAssetID(fixedAssetID);
            string transferReason = maskedTextBox1.Text;
            string notes = maskedTextBox3.Text;
            int toDepartmentID = ((KeyValuePair<int, string>)comboBox4.SelectedItem).Key;
            int toEmployeeID = ((KeyValuePair<int, string>)comboBox3.SelectedItem).Key;

            
            if ( string.IsNullOrEmpty(transferReason) )
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return; // Dừng lại nếu có ít nhất một biến không hợp lệ
            }

            // Gọi phương thức từ BUS để thêm bản ghi
            bool result = bUSTransfer.UpdateHistoryTransferAsset(id,fixedAssetID, transferDate, fromDepartmentID, toDepartmentID, transferReason, notes, fromEmployeeID, toEmployeeID);

            // Hiển thị thông báo kết quả
            if (result)
            {
                MessageBox.Show("Bản ghi đã được thêm thành công.");
                button12_Click(sender, e);

                // Gọi phương thức để tải lại dữ liệu hoặc làm sạch các trường dữ liệu
            }
            else
            {
                MessageBox.Show("Đã xảy ra lỗi khi thêm bản ghi.");
            }
            LoadTransfer(FromDate, ToDate);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            id = string.Empty;
            maskedTextBox3.Text = "";
            maskedTextBox1.Text = "";
            LoadDataIntoComboBox48();
            LoadDataIntoComboBox3();
            LoadDataIntoComboBox9();
            LoadDataIntoComboBox5(searchKeyword, assetTypeID, departmentID, status);
            comboBox5.Enabled = true;

            dateTimePicker3.Value = DateTime.Today;
            button4.Visible = false;

            button8.Visible = false;
            button9.Visible = false;
            button2.Visible = true;
            button12.Visible = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            HistoryTransferAsset historyTransferAsset = bUSTransfer.GetHistoryTransferAssetById(id);

            int fromDepartmentID = historyTransferAsset.FromDepartmentID;
            int fromEmployeeID = historyTransferAsset.FromEmployeeID;
            // Kiểm tra kết quả của hộp thoại
            if (result == DialogResult.Yes)
            {
                // Xóa bản ghi từ ID
                if (bUSTransfer.DeleteHistoryTransferAsset(id, fromDepartmentID, fromEmployeeID))
                {
                    MessageBox.Show("Xóa thông tin thành công.");
                    LoadTransfer(FromDate, ToDate);
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
            DataTable dataTable = bUSTransfer.GetAllTransfer(FromDate, ToDate);

            // Gọi phương thức từ lớp BUS để xuất dữ liệu sang Excel
            bUSTransfer.ExportToExcel(dataTable);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Tạo một tài liệu mới
            Document document = new Document();

            // Tạo một save file dialog
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
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

                    Paragraph title = new Paragraph("Phiếu Chuyển Giao", titlefont1);
                    paragraph.Add("\n");
                    paragraph.Add("\n");
               
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    paragraph.Add(new Chunk("Mã chuyển giao : ", titlefont));
                    paragraph.Add(new Chunk("TF0" + id, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Tên tài sản chuyển giao : ", titlefont));
                    paragraph.Add(new Chunk(comboBox5.Text, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Mã tài sản chuyển giao : ", titlefont));
                    paragraph.Add(new Chunk("AS0" + (int)comboBox5.SelectedValue, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Lý do chuyển giao ", titlefont));
                    paragraph.Add(new Chunk(maskedTextBox1.Text, font));
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Phòng ban giao : ", titlefont));
                    paragraph.Add(new Chunk(comboBox1.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Nhân viên giao : ", titlefont));
                    paragraph.Add(new Chunk(comboBox2.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Phòng ban nhận : ", titlefont));
                    paragraph.Add(new Chunk(comboBox4.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Nhân viên nhận : ", titlefont));
                    paragraph.Add(new Chunk(comboBox3.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Ghi chú chuyển giao : ", titlefont));
                    paragraph.Add(new Chunk(maskedTextBox3.Text, font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("Ngày chuyển giao : ", titlefont));
                    paragraph.Add(new Chunk(dateTimePicker3.Value.ToShortDateString(), font));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n");
                    paragraph.Add(new Chunk("                                  _________________________________________", titlefont));
                    paragraph.Add("\n"); paragraph.Add("\n");
                    paragraph.Add("\n"); paragraph.Add("\n");
 
                    paragraph.Add(new Chunk("                                Đơn vị giao                                                 Đơn vị nhận", font));

                    document.Add(paragraph);

                    // Đóng tài liệu
                    document.Close();

                    MessageBox.Show("PDF exported successfully to: " + outputFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}
