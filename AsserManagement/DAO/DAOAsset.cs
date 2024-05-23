using DocumentFormat.OpenXml.Bibliography;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace AsserManagement
{
    public class DAOAsset
    {
        private DatabaseManager dbManager;
        private DataTable dataTable; // Khai báo biến searchData ở mức độ của lớp
        private DataTable originalData; // Khai báo biến searchData ở mức độ của lớp

        public DAOAsset()
        {
            dbManager = new DatabaseManager();
            dataTable = new DataTable();
            originalData = new DataTable();
        }



        public DataTable GetAssetData(string searchKeyword, int assetTypeID, int departmentID, int employeeID, string status, DateTime FromDate, DateTime ToDate, DateTime FromWarranty, DateTime ToWarranty)
        {
            if (dbManager.OpenConnection())
            {
                dataTable.Clear();

                string query = "SELECT f.FixedAssetID AS ID, f.AssetName AS Name, a.AssetTypeName AS Type, d.DepartmentName AS Department, CONCAT(e.LastName, ' ', e.FirstName) AS Employee, f.Value, f.PurchaseDate AS Date, f.WarrantyDate AS Warranty, f.Status " +
                                "FROM fixedassets f " +
                                "JOIN assettypes a ON f.AssetTypeID = a.AssetTypeID " +
                                "JOIN departments d ON f.DepartmentID = d.DepartmentID " +
                                "JOIN employees e ON f.EmployeeID = e.EmployeeID " +
                                "WHERE 1=1"; // Điều kiện mặc định

                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    query += $" AND f.AssetName LIKE '%{searchKeyword}%'";
                }

                if (assetTypeID != 0)
                {
                    query += $" AND f.AssetTypeID = {assetTypeID}";
                }

                if (departmentID != 0)
                {
                    query += $" AND f.DepartmentID = {departmentID}";
                }

                if (employeeID != 0)
                {
                    query += $" AND f.EmployeeID = {employeeID}";
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query += $" AND f.Status = '{status}'";
                }
                if (FromDate != DateTime.MinValue && ToDate != DateTime.MinValue)
                {
                    query += $" AND f.PurchaseDate >= '{FromDate:yyyy-MM-dd}' " +
                             $" AND f.PurchaseDate <= '{ToDate:yyyy-MM-dd}' ";
                }
                if (FromWarranty != DateTime.MinValue && ToWarranty != DateTime.MinValue)
                {
                    query += $" AND f.WarrantyDate >= '{FromWarranty:yyyy-MM-dd}' " +
                             $" AND f.WarrantyDate <= '{ToWarranty:yyyy-MM-dd}' ";
                }

                query += " ORDER BY f.FixedAssetID ASC";


                var dataReader = dbManager.ExecuteQuery(query);
                dataTable.Load(dataReader);
                dbManager.CloseConnection();
                return dataTable;
            }
            return null;
        }


        public DataTable GetAssetDataForPage(int pageNumber, int pageSize)
        {
            DataTable originalData = dataTable;

            if (originalData != null)
            {
                int startIndex = (pageNumber - 1) * pageSize;
                DataTable pageData = originalData.Clone();
                for (int i = startIndex; i < Math.Min(startIndex + pageSize, originalData.Rows.Count); i++)
                {
                    pageData.ImportRow(originalData.Rows[i]);
                }
                return pageData;
            }
            return null;
        }

        public int GetTotalPages(int pageSize, string searchKeyword, int assetTypeID, int departmentID, int employeeID, string status, DateTime FromDate, DateTime ToDate, DateTime FromWarranty, DateTime ToWarranty)
        {
            originalData = GetAssetData(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
            if (originalData != null)
            {
                int totalRows = originalData.Rows.Count;
                int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);

                // Đảm bảo totalPages luôn lớn hơn hoặc bằng 1
                if (totalPages < 1)
                {
                    totalPages = 1;
                }

                return totalPages;
            }

            return 1;
        }


        public int DeleteAsset(int assetID)
        {
            try
            {
                if (dbManager.OpenConnection())
                {
                    string query = $"DELETE FROM fixedassets WHERE FixedAssetID = {assetID}";
                    MySqlCommand command = new MySqlCommand(query, dbManager.Connection);
                    int rowsAffected = command.ExecuteNonQuery();
                    dbManager.CloseConnection();
                    return rowsAffected;
                }
                else
                {
                    return -1; // Hoặc một giá trị đại diện cho lỗi kết nối
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                return -1; // Hoặc một giá trị đại diện cho lỗi khác
            }
        }
        public DataTable GetAllTransfer(decimal fixedAssetID)
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = $"SELECT ht.HistoryTransferAssetID AS ID, fa.AssetName AS AssetName, ht.TransferDate AS Date, ht.TransferReason AS Reason, d1.DepartmentName AS FromDepartment, CONCAT(e1.LastName, ' ', e1.FirstName) AS FromEmployee, d2.DepartmentName AS ToDepartment, CONCAT(e2.LastName, ' ', e2.FirstName) AS ToEmployee, ht.Notes AS Notes FROM historytransferasset ht INNER JOIN fixedassets fa ON ht.FixedAssetID = fa.FixedAssetID INNER JOIN departments d1 ON ht.FromDepartmentID = d1.DepartmentID INNER JOIN departments d2 ON ht.ToDepartmentID = d2.DepartmentID INNER JOIN employees e1 ON ht.FromEmployeeID = e1.EmployeeID INNER JOIN employees e2 ON ht.ToEmployeeID = e2.EmployeeID WHERE ht.FixedAssetID = {fixedAssetID} ORDER BY ht.TransferDate DESC";

                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);

                // Thêm tham số truy vấn
                cmd.Parameters.AddWithValue("@FixedAssetID", fixedAssetID);

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
        public DataTable GetAllRepair(decimal fixedAssetID)
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = $"SELECT rm.RepairID AS 'ID', fa.AssetName AS 'AssetName', rm.RepairDate AS 'Date', rm.Description AS 'Description', rm.RepairCost AS 'Cost', d.DepartmentName AS 'Department', CONCAT(e.LastName, ' ', e.FirstName) AS 'Employee', rm.Status AS 'Status', rm.NextRepairDate AS 'NextDate' FROM repairsandmaintenance rm INNER JOIN fixedassets fa ON rm.FixedAssetID = fa.FixedAssetID INNER JOIN departments d ON rm.DepartmentID = d.DepartmentID INNER JOIN employees e ON rm.EmployeeID = e.EmployeeID WHERE rm.FixedAssetID = {fixedAssetID} ORDER BY rm.RepairDate DESC";

                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                cmd.Parameters.AddWithValue("@FixedAssetID", fixedAssetID);

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
        public DataTable GetAllDisposal(decimal fixedAssetID)
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
                query += $" WHERE d.FixedAssetID = {fixedAssetID}";
                query += " ORDER BY d.DisposalDate DESC";
                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                cmd.Parameters.AddWithValue("@FixedAssetID", fixedAssetID);
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



        // Other data access methods...
    }
}
