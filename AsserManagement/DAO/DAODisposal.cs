using AsserManagement.DTO;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsserManagement.DAO
{
    internal class DAODisposal
    {
        private DatabaseManager dbManager;
        public DAODisposal()
        {
            dbManager = new DatabaseManager();
        }
        public DataTable GetAllDisposal(DateTime FromDate, DateTime ToDate)
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = "SELECT " +
                               "    d.DisposalID AS ID, " +
                               "    fa.AssetName AS AssetName, " +
                               "    d.DisposalDate AS Date, " +
                               "    d.Reason AS DisposalReason, " +
                               "    d.SaleValue AS SaleValue, " +
                               "    d2.DepartmentName AS DepartmentName, " +
                               "    CONCAT(e.LastName, ' ', e.FirstName) AS EmployeeName " +
                               "FROM " +
                               "    disposal d " +
                               "INNER JOIN " +
                               "    fixedassets fa ON d.FixedAssetID = fa.FixedAssetID " +
                               "INNER JOIN " +
                               "    departments d2 ON d.DepartmentID = d2.DepartmentID " +
                               "INNER JOIN " +
                               "    employees e ON d.EmployeeID = e.EmployeeID";
                if (FromDate != DateTime.MinValue && ToDate != DateTime.MinValue)
                {
                    query += $" AND d.DisposalDate >= '{FromDate:yyyy-MM-dd}' " +
                             $" AND d.DisposalDate <= '{ToDate:yyyy-MM-dd}' ";
                }

                query += " ORDER BY d.DisposalDate DESC";
                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dataTable);

            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return dataTable;
        }
        public List<KeyValuePair<int, string>> GetAssetKeyValuePairList(string searchKeyword, int assetTypeID, int departmentID, string status)
        {
            List<KeyValuePair<int, string>> keyValuePairs = new List<KeyValuePair<int, string>>();
            string query = "SELECT FixedAssetID, AssetName FROM fixedassets WHERE 1=1"; // Điều kiện mặc định

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query += $" AND AssetName LIKE '%{searchKeyword}%'";
            }

            if (assetTypeID > 0)
            {
                query += $" AND AssetTypeID = {assetTypeID}";
            }

            if (departmentID > 0)
            {
                query += $" AND DepartmentID = {departmentID}";
            }

            if (!string.IsNullOrEmpty(status))
            {
                query += $" AND Status = '{status}'";
            }

            query += " ORDER BY FixedAssetID ASC";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["FixedAssetID"]);
                        string name = $"{reader["FixedAssetID"]} - {reader["AssetName"]}";
                        keyValuePairs.Add(new KeyValuePair<int, string>(id, name));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return keyValuePairs;
        }
        public List<KeyValuePair<int, string>> GetDepartmentKeyValuePairList()
        {
            List<KeyValuePair<int, string>> keyValuePairs = new List<KeyValuePair<int, string>>();
            string query = "SELECT DepartmentID, DepartmentName FROM departments";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["DepartmentID"]);
                        string name = reader["DepartmentName"].ToString();
                        keyValuePairs.Add(new KeyValuePair<int, string>(id, name));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return keyValuePairs;
        }
        public List<KeyValuePair<int, string>> GetTypeKeyValuePairList()
        {
            List<KeyValuePair<int, string>> keyValuePairs = new List<KeyValuePair<int, string>>();
            string query = "SELECT AssetTypeID, AssetTypeName FROM assettypes";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["AssetTypeID"]);
                        string name = reader["AssetTypeName"].ToString();
                        keyValuePairs.Add(new KeyValuePair<int, string>(id, name));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return keyValuePairs;
        }
        public List<KeyValuePair<int, string>> GetEmployeeKeyValuePairList()
        {
            List<KeyValuePair<int, string>> keyValuePairs = new List<KeyValuePair<int, string>>();
            string query = "SELECT EmployeeID, LastName, FirstName FROM employees";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["EmployeeID"]);
                        string name = $"{reader["EmployeeID"]} - {reader["LastName"]} {reader["FirstName"]}";
                        keyValuePairs.Add(new KeyValuePair<int, string>(id, name));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return keyValuePairs;
        }
        public bool AddDisposal(int fixedAssetID, DateTime disposalDate, string reason, decimal saleValue, int departmentID, int employeeID)
        {
            try
            {
                dbManager.OpenConnection();

                // Thêm dữ liệu vào bảng disposal
                string insertDisposalQuery = $"INSERT INTO disposal (FixedAssetID, DisposalDate, Reason, SaleValue, DepartmentID, EmployeeID) " +
                               $"VALUES ('{fixedAssetID}', '{disposalDate:yyyy-MM-dd}', '{reason}', '{saleValue}', '{departmentID}', '{employeeID}')";
                MySqlCommand insertDisposalCmd = new MySqlCommand(insertDisposalQuery, dbManager.Connection);
                insertDisposalCmd.ExecuteNonQuery();
                string updateFixedAssetsQuery = $"UPDATE fixedassets SET Status = '{"Đã thanh lý"}' WHERE FixedAssetID = '{fixedAssetID}'";
                MySqlCommand updateFixedAssetsCmd = new MySqlCommand(updateFixedAssetsQuery, dbManager.Connection);
                updateFixedAssetsCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return false;
            }
            finally
            {
                dbManager.CloseConnection();
            }
        }
        public Disposal GetDisposalById(string id)
        {
            Disposal disposal = null;
            string query = $"SELECT FixedAssetID, DisposalDate, Reason, SaleValue, DepartmentID, EmployeeID FROM disposal WHERE DisposalID = {id}";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int fixedAssetID = Convert.ToInt32(reader["FixedAssetID"]);
                        DateTime disposalDate = Convert.ToDateTime(reader["DisposalDate"]);
                        string reason = reader["Reason"].ToString();
                        decimal saleValue = Convert.ToDecimal(reader["SaleValue"]);
                        int departmentID = Convert.ToInt32(reader["DepartmentID"]);
                        int employeeID = Convert.ToInt32(reader["EmployeeID"]);

                        disposal = new Disposal
                        {
                            FixedAssetID = fixedAssetID,
                            DisposalDate = disposalDate,
                            Reason = reason,
                            SaleValue = saleValue,
                            DepartmentID = departmentID,
                            EmployeeID = employeeID
                        };
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return disposal;
        }
        public bool UpdateDisposal(string disposalID, int fixedAssetID, DateTime disposalDate, string reason, decimal saleValue, int departmentID, int employeeID)
        {
            try
            {
                // Chuỗi truy vấn SQL cập nhật thông tin bản ghi Disposal
                string query = $"UPDATE disposal SET FixedAssetID = {fixedAssetID}, DisposalDate = '{disposalDate:yyyy-MM-dd}', Reason = '{reason}', SaleValue = {saleValue}, DepartmentID = {departmentID}, EmployeeID = {employeeID} WHERE DisposalID = {disposalID}";

                // Mở kết nối đến cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    // Tạo và thực thi command để cập nhật dữ liệu
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Đóng kết nối
                    dbManager.CloseConnection();
                    // Trả về true nếu có ít nhất một hàng bị ảnh hưởng (đã cập nhật thành công)
                    return rowsAffected > 0;
                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                return false;
            }
        }
        public bool DeleteDisposal(string disposalID)
        {
            try
            {
                // Mở kết nối đến cơ sở dữ liệu
                if (dbManager.OpenConnection())
                {
                    // Truy vấn FixedAssetID từ bảng disposal
                    string getDisposalInfoQuery = $"SELECT FixedAssetID FROM disposal WHERE DisposalID = {disposalID}";
                    MySqlCommand getRepairInfoCmd = new MySqlCommand(getDisposalInfoQuery, dbManager.Connection);
                    MySqlDataReader reader = getRepairInfoCmd.ExecuteReader();

                    int fixedAssetID = -1;

                    if (reader.Read())
                    {
                        fixedAssetID = reader.GetInt32(0);
                    }

                    reader.Close();

                    if (fixedAssetID != -1)
                    {
                        // Cập nhật trạng thái của fixed asset trong bảng fixedassets
                        string updateFixedAssetsQuery = $"UPDATE fixedassets SET Status = 'Cần thanh lý' WHERE FixedAssetID = {fixedAssetID}";
                        MySqlCommand updateFixedAssetsCmd = new MySqlCommand(updateFixedAssetsQuery, dbManager.Connection);
                        updateFixedAssetsCmd.ExecuteNonQuery();
                    }

                    // Xóa bản ghi trong bảng disposal
                    string deleteDisposalQuery = $"DELETE FROM disposal WHERE DisposalID = {disposalID}";
                    MySqlCommand deleteRepairsCmd = new MySqlCommand(deleteDisposalQuery, dbManager.Connection);
                    int rowsAffected = deleteRepairsCmd.ExecuteNonQuery();

                    // Đóng kết nối
                    dbManager.CloseConnection();

                    // Trả về true nếu có ít nhất một hàng bị ảnh hưởng (bản ghi đã được xóa thành công)
                    return rowsAffected > 0;
                }
                else
                {
                    MessageBox.Show("Không thể kết nối tới cơ sở dữ liệu.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                return false;
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

                    // Hiển thị hộp thoại lưu tệp
                    using (var saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                        saveFileDialog.FileName = "Disposal_list_Exported";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            workbook.SaveAs(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
