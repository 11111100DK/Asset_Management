using AsserManagement.DTO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsserManagement.DAO
{

    public class DAORepair
    {
        private DatabaseManager dbManager;
        public DAORepair()
        {
            dbManager = new DatabaseManager();
        }
        public DataTable GetAllRepair(DateTime FromDate, DateTime ToDate )
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = "SELECT rm.RepairID AS 'ID', \r\n" +
                               "       fa.AssetName AS 'AssetName', \r\n" +
                               "       rm.RepairDate AS 'Date', \r\n" +
                               "       rm.Description AS 'Description', \r\n" +
                               "       rm.RepairCost AS 'Cost', \r\n" +
                               "       d.DepartmentName AS 'Department', \r\n" +
                               "       CONCAT(e.LastName, ' ', e.FirstName) AS 'Employee', \r\n" +
                               "       rm.Status AS 'Status', \r\n" +
                               "       rm.NextRepairDate AS 'NextDate' \r\n" +
                               "FROM repairsandmaintenance rm\r\n" +
                               "INNER JOIN fixedassets fa ON rm.FixedAssetID = fa.FixedAssetID\r\n" +
                               "INNER JOIN departments d ON rm.DepartmentID = d.DepartmentID\r\n" +
                               "INNER JOIN employees e ON rm.EmployeeID = e.EmployeeID ";
                if (FromDate != DateTime.MinValue && ToDate != DateTime.MinValue)
                {
                    query += $" AND rm.RepairDate >= '{FromDate:yyyy-MM-dd}' " +
                             $" AND rm.RepairDate <= '{ToDate:yyyy-MM-dd}' ";
                }

                query += " ORDER BY rm.RepairDate DESC";
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
        public bool AddRepair(int fixedAssetID, DateTime repairDate, string description, decimal repairCost, int departmentID, int employeeID, string status, DateTime nextRepairDate)
        {
            try
            {
                dbManager.OpenConnection();

                // Truy vấn WarrantyDate từ bảng fixedassets
                string getWarrantyDateQuery = $"SELECT WarrantyDate FROM fixedassets WHERE FixedAssetID = '{fixedAssetID}'";
                MySqlCommand getWarrantyDateCmd = new MySqlCommand(getWarrantyDateQuery, dbManager.Connection);
                DateTime warrantyDate = (DateTime)getWarrantyDateCmd.ExecuteScalar();

                // Kiểm tra và cập nhật status trong bảng fixedassets
                string astatus = "";
                if (status == "Tiến hành bảo trì")
                {
                    astatus = "Đang bảo trì";
                }
                else if (status == "Bảo trì hoàn thành")
                {
                    astatus = "Đang sử dụng";
                }
                else if (status == "Không thể bảo trì")
                {
                    astatus = "Cần thanh lý";
                }
                string updateFixedAssetsQuery = $"UPDATE fixedassets SET Status = '{astatus}', WarrantyDate = '{nextRepairDate:yyyy-MM-dd}' WHERE FixedAssetID = '{fixedAssetID}'";
                MySqlCommand updateFixedAssetsCmd = new MySqlCommand(updateFixedAssetsQuery, dbManager.Connection);
                updateFixedAssetsCmd.ExecuteNonQuery();

                // Thêm dữ liệu vào bảng repairsandmaintenance
                string insertRepairsQuery = $"INSERT INTO repairsandmaintenance (FixedAssetID, RepairDate, Description, RepairCost, DepartmentID, EmployeeID, Status, NextRepairDate, WarrantyDate) " +
                               $"VALUES ('{fixedAssetID}', '{repairDate:yyyy-MM-dd}', '{description}', '{repairCost}', '{departmentID}', '{employeeID}', '{status}', '{nextRepairDate:yyyy-MM-dd}', '{warrantyDate:yyyy-MM-dd}')";
                MySqlCommand insertRepairsCmd = new MySqlCommand(insertRepairsQuery, dbManager.Connection);
                insertRepairsCmd.ExecuteNonQuery();

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

        public RepairsAndMaintenance GetRepairsAndMaintenanceById(string id)
        {
            RepairsAndMaintenance repair = null;
            string query = $"SELECT FixedAssetID, RepairDate, Description, RepairCost, DepartmentID, EmployeeID, Status, NextRepairDate FROM repairsandmaintenance WHERE RepairID = {id}";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int fixedAssetID = Convert.ToInt32(reader["FixedAssetID"]);
                        DateTime repairDate = Convert.ToDateTime(reader["RepairDate"]);
                        string description = reader["Description"].ToString();
                        decimal repairCost = Convert.ToDecimal(reader["RepairCost"]);
                        int departmentID = Convert.ToInt32(reader["DepartmentID"]);
                        int employeeID = Convert.ToInt32(reader["EmployeeID"]);
                        string status = reader["Status"].ToString();
                        DateTime nextRepairDate = Convert.ToDateTime(reader["NextRepairDate"]);

                        repair = new RepairsAndMaintenance
                        {
                            FixedAssetID = fixedAssetID,
                            RepairDate = repairDate,
                            Description = description,
                            RepairCost = repairCost,
                            DepartmentID = departmentID,
                            EmployeeID = employeeID,
                            Status = status,
                            NextRepairDate = nextRepairDate
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

            return repair;
        }
        public bool UpdateRepairsAndMaintenance(string id, int fixedAssetID, DateTime repairDate, string description, decimal repairCost, int departmentID, int employeeID, string status, DateTime nextRepairDate)
        {
            string query = $"UPDATE repairsandmaintenance SET FixedAssetID = '{fixedAssetID}', RepairDate = '{repairDate:yyyy-MM-dd}', Description = '{description}', RepairCost = '{repairCost}', DepartmentID = '{departmentID}', EmployeeID = '{employeeID}', Status = '{status}', NextRepairDate = '{nextRepairDate:yyyy-MM-dd}' WHERE RepairID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
                    string astatus = "";
                    // Kiểm tra nếu status là "Tiến hành bảo trì" thì sửa đổi thành "Đang bảo trì" cả ở bảng fixedassets và repairsandmaintenance
                    if (status == "Tiến hành bảo trì")
                    {
                        astatus = "Đang bảo trì";
                    }
                    else if (status == "Bảo trì hoàn thành")
                    {
                        astatus = "Đang sử dụng";
                    }
                    else if (status == "Không thể bảo trì")
                    {
                        astatus = "Cần thanh lý";
                    }
                    // Cập nhật status trong bảng fixedassets
                    string updateFixedAssetsQuery = $"UPDATE fixedassets SET Status = '{astatus}', WarrantyDate = '{nextRepairDate:yyyy-MM-dd}' WHERE FixedAssetID = '{fixedAssetID}'";
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
        public bool DeleteRepairsAndMaintenance(string id)
        {
            try
            {
                dbManager.OpenConnection();

                // Truy vấn WarrantyDate và FixedAssetID từ bảng repairsandmaintenance
                string getRepairInfoQuery = $"SELECT FixedAssetID, WarrantyDate FROM repairsandmaintenance WHERE RepairID = '{id}'";
                MySqlCommand getRepairInfoCmd = new MySqlCommand(getRepairInfoQuery, dbManager.Connection);
                MySqlDataReader reader = getRepairInfoCmd.ExecuteReader();

                DateTime warrantyDate = DateTime.MinValue;
                int fixedAssetID = -1;

                if (reader.Read())
                {
                    fixedAssetID = reader.GetInt32("FixedAssetID");
                    warrantyDate = reader.GetDateTime("WarrantyDate");
                }

                reader.Close();

                if (fixedAssetID != -1)
                {
                    // Cập nhật trạng thái và giá trị WarrantyDate trong bảng fixedassets
                    string updateFixedAssetsQuery = $"UPDATE fixedassets SET Status = 'Đang sử dụng', WarrantyDate = '{warrantyDate:yyyy-MM-dd}' WHERE FixedAssetID = '{fixedAssetID}'";
                    MySqlCommand updateFixedAssetsCmd = new MySqlCommand(updateFixedAssetsQuery, dbManager.Connection);
                    updateFixedAssetsCmd.ExecuteNonQuery();
                }

                // Xóa bản ghi trong bảng repairsandmaintenance
                string deleteRepairsQuery = $"DELETE FROM repairsandmaintenance WHERE RepairID = '{id}'";
                MySqlCommand deleteRepairsCmd = new MySqlCommand(deleteRepairsQuery, dbManager.Connection);
                int rowsAffected = deleteRepairsCmd.ExecuteNonQuery();

                return rowsAffected > 0;
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
                        saveFileDialog.FileName = "Repair_and_Maintenance_Exported";

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
