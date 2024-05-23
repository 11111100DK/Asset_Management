using AsserManagement.DTO;
using ClosedXML.Excel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsserManagement.DAO
{
    internal class DAOTransfer
    {
        private DatabaseManager dbManager;
        public DAOTransfer()
        {
            dbManager = new DatabaseManager();
        }
        public DataTable GetAllTransfer(DateTime FromDate, DateTime ToDate)
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = "SELECT " +
                               "    ht.HistoryTransferAssetID AS ID, " +
                               "    fa.AssetName AS AssetName, " +
                               "    ht.TransferDate AS Date, " +
                               "    ht.TransferReason AS Reason, " +
                               "    d1.DepartmentName AS FromDepartment, " +
                               "    CONCAT(e1.LastName, ' ', e1.FirstName) AS FromEmployee, " +
                               "    d2.DepartmentName AS ToDepartment, " +
                               "    CONCAT(e2.LastName, ' ', e2.FirstName) AS ToEmployee, " +
                               "    ht.Notes AS Notes " +
                               "FROM " +
                               "    historytransferasset ht " +
                               "INNER JOIN " +
                               "    fixedassets fa ON ht.FixedAssetID = fa.FixedAssetID " +
                               "INNER JOIN " +
                               "    departments d1 ON ht.FromDepartmentID = d1.DepartmentID " +
                               "INNER JOIN " +
                               "    departments d2 ON ht.ToDepartmentID = d2.DepartmentID " +
                               "INNER JOIN " +
                               "    employees e1 ON ht.FromEmployeeID = e1.EmployeeID " +
                               "INNER JOIN " +
                               "    employees e2 ON ht.ToEmployeeID = e2.EmployeeID ";
                if (FromDate != DateTime.MinValue && ToDate != DateTime.MinValue)
                {
                    query += $" AND ht.TransferDate >= '{FromDate:yyyy-MM-dd}' " +
                             $" AND ht.TransferDate <= '{ToDate:yyyy-MM-dd}' ";
                }

                query += " ORDER BY ht.TransferDate DESC";
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
        public int GetDepartmentIDByFixedAssetID(int fixedAssetID)
        {
            int departmentID = -1; // Giá trị mặc định nếu không tìm thấy

            try
            {
                // Viết câu truy vấn SQL để lấy DepartmentID từ bảng fixedassets dựa trên FixedAssetID
                string query = $"SELECT DepartmentID FROM fixedassets WHERE FixedAssetID = {fixedAssetID}";

                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        departmentID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return departmentID;
        }

        // Phương thức để lấy EmployeeID từ bảng fixedassets dựa trên FixedAssetID
        public int GetEmployeeIDByFixedAssetID(int fixedAssetID)
        {
            int employeeID = -1; // Giá trị mặc định nếu không tìm thấy

            try
            {
                // Viết câu truy vấn SQL để lấy EmployeeID từ bảng fixedassets dựa trên FixedAssetID
                string query = $"SELECT EmployeeID FROM fixedassets WHERE FixedAssetID = {fixedAssetID}";

                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        employeeID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return employeeID;
        }

        // Phương thức để lấy DepartmentName từ bảng departments dựa trên DepartmentID
        public string GetDepartmentNameByID(int departmentID)
        {
            string departmentName = string.Empty;

            try
            {
                // Viết câu truy vấn SQL để lấy DepartmentName từ bảng departments dựa trên DepartmentID
                string query = $"SELECT DepartmentName FROM departments WHERE DepartmentID = {departmentID}";

                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        departmentName = Convert.ToString(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return departmentName;
        }

        // Phương thức để lấy EmployeeName từ bảng employees dựa vào EmployeeID
        public string GetEmployeeNameByID(int employeeID)
        {
            string employeeName = string.Empty;

            try
            {
                // Viết câu truy vấn SQL để lấy LastName và FirstName từ bảng employees dựa vào EmployeeID
                string query = $"SELECT CONCAT(EmployeeID, ' - ', LastName, ' ', FirstName) AS EmployeeName FROM employees WHERE EmployeeID = {employeeID}";

                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        employeeName = Convert.ToString(result);
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return employeeName;
        }
        public bool AddTransfer(int fixedAssetID, DateTime transferDate, int fromDepartmentID, int toDepartmentID, string transferReason, string notes, int fromEmployeeID, int toEmployeeID)
        {
            try
            {
                dbManager.OpenConnection();
                string updateFixedAssetsQuery = $"UPDATE fixedassets SET DepartmentID = '{toDepartmentID}', EmployeeID = '{toEmployeeID}' WHERE FixedAssetID = '{fixedAssetID}'";
                MySqlCommand updateFixedAssetsCmd = new MySqlCommand(updateFixedAssetsQuery, dbManager.Connection);
                updateFixedAssetsCmd.ExecuteNonQuery();
                // Thêm dữ liệu vào bảng historytransferasset
                string insertTransferQuery = $"INSERT INTO historytransferasset (FixedAssetID, TransferDate, FromDepartmentID, ToDepartmentID, TransferReason, Notes, FromEmployeeID, ToEmployeeID) " +
                               $"VALUES ('{fixedAssetID}', '{transferDate:yyyy-MM-dd}', '{fromDepartmentID}', '{toDepartmentID}', '{transferReason}', '{notes}', '{fromEmployeeID}', '{toEmployeeID}')";
                MySqlCommand insertTransferCmd = new MySqlCommand(insertTransferQuery, dbManager.Connection);
                insertTransferCmd.ExecuteNonQuery();

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
        public HistoryTransferAsset GetHistoryTransferAssetById(string id)
        {
            HistoryTransferAsset transferAsset = null;
            string query = $"SELECT FixedAssetID, TransferDate, FromDepartmentID, ToDepartmentID, TransferReason, Notes, FromEmployeeID, ToEmployeeID FROM historytransferasset WHERE HistoryTransferAssetID = {id}";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int fixedAssetID = Convert.ToInt32(reader["FixedAssetID"]);
                        DateTime transferDate = Convert.ToDateTime(reader["TransferDate"]);
                        int fromDepartmentID = Convert.ToInt32(reader["FromDepartmentID"]);
                        int toDepartmentID = Convert.ToInt32(reader["ToDepartmentID"]);
                        string transferReason = reader["TransferReason"].ToString();
                        string notes = reader["Notes"].ToString();
                        int fromEmployeeID = reader["FromEmployeeID"] != DBNull.Value ? Convert.ToInt32(reader["FromEmployeeID"]) : 0;
                        int toEmployeeID = reader["ToEmployeeID"] != DBNull.Value ? Convert.ToInt32(reader["ToEmployeeID"]) : 0;

                        transferAsset = new HistoryTransferAsset
                        {
                            FixedAssetID = fixedAssetID,
                            TransferDate = transferDate,
                            FromDepartmentID = fromDepartmentID,
                            ToDepartmentID = toDepartmentID,
                            TransferReason = transferReason,
                            Notes = notes,
                            FromEmployeeID = fromEmployeeID,
                            ToEmployeeID = toEmployeeID
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

            return transferAsset;
        }
        public bool UpdateHistoryTransferAsset(string id, int fixedAssetID, DateTime transferDate, int fromDepartmentID, int toDepartmentID, string transferReason, string notes, int fromEmployeeID, int toEmployeeID)
        {
            string query = $"UPDATE historytransferasset SET FixedAssetID = '{fixedAssetID}', TransferDate = '{transferDate:yyyy-MM-dd}', ToDepartmentID = '{toDepartmentID}', TransferReason = '{transferReason}', Notes = '{notes}', ToEmployeeID = '{toEmployeeID}' WHERE HistoryTransferAssetID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
                    string updateFixedAssetsQuery = $"UPDATE fixedassets SET DepartmentID = '{toDepartmentID}', EmployeeID = '{toEmployeeID}' WHERE FixedAssetID = '{fixedAssetID}'";
                    MySqlCommand updateFixedAssetsCmd = new MySqlCommand(updateFixedAssetsQuery, dbManager.Connection);
                    updateFixedAssetsCmd.ExecuteNonQuery();
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    int rowsAffected = cmd.ExecuteNonQuery();
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
            finally
            {
                dbManager.CloseConnection();
            }
        }
        public bool DeleteHistoryTransferAsset(string id, int fromDepartmentID, int fromEmployeeID)
        {
            try
            {
                if (dbManager.OpenConnection())
                {
                    // Truy vấn thông tin về FixedAssetID từ bảng historytransferasset
                    string getFixedAssetIDQuery = $"SELECT FixedAssetID FROM historytransferasset WHERE HistoryTransferAssetID = '{id}'";
                    MySqlCommand getFixedAssetIDCmd = new MySqlCommand(getFixedAssetIDQuery, dbManager.Connection);
                    int fixedAssetID = Convert.ToInt32(getFixedAssetIDCmd.ExecuteScalar());

                    // Xóa bản ghi trong bảng historytransferasset
                    string deleteTransferQuery = $"DELETE FROM historytransferasset WHERE HistoryTransferAssetID = '{id}'";
                    MySqlCommand deleteTransferCmd = new MySqlCommand(deleteTransferQuery, dbManager.Connection);
                    int rowsAffected = deleteTransferCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Nếu xóa thành công, cập nhật trạng thái và giá trị WarrantyDate trong bảng fixedassets
                        string updateFixedAssetsQuery = $"UPDATE fixedassets SET DepartmentID = '{fromDepartmentID}', EmployeeID = '{fromEmployeeID}' WHERE FixedAssetID = '{fixedAssetID}'";
                        MySqlCommand updateFixedAssetsCmd = new MySqlCommand(updateFixedAssetsQuery, dbManager.Connection);
                        updateFixedAssetsCmd.ExecuteNonQuery();

                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Không có bản ghi nào được xóa.");
                        return false;
                    }
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
            finally
            {
                dbManager.CloseConnection();
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
                        saveFileDialog.FileName = "Transfer_list_Exported";

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
