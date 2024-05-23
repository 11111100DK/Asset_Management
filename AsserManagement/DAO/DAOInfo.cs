using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AsserManagement
{
    public class DAOInfo
    {
        private DatabaseManager dbManager;

        public DAOInfo()
        {
            dbManager = new DatabaseManager();
        }

        public DataTable GetAllAssetTypes()
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = "SELECT AssetTypeID as 'ID', AssetTypeName as 'Name', Description as 'Description' FROM assettypes"; // Điều chỉnh tên cột ở đây
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
        public DataTable GetAllDepartments()
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = "SELECT DepartmentID as 'ID', DepartmentName as 'Name', Description as 'Description' FROM departments"; // Điều chỉnh tên cột ở đây
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
        public DataTable GetAllEmployees()
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = "SELECT EmployeeID as 'ID', CONCAT(LastName, ' ', FirstName) as 'Name', Position as 'Position' FROM employees"; // Điều chỉnh tên cột ở đây
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
        public bool AddAssetType(string name, string description)
        {
            try
            {
                dbManager.OpenConnection();
                string query = $"INSERT INTO assettypes (AssetTypeName, Description) VALUES ('{name}', '{description}')";
                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                cmd.ExecuteNonQuery();
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
        public bool AddDepartment(string name, string description)
        {
            try
            {
                dbManager.OpenConnection();
                string query = $"INSERT INTO departments (DepartmentName, Description) VALUES ('{name}', '{description}')";
                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                cmd.ExecuteNonQuery();
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
        public bool AddEmployee(string last, string first, string position, int idd)
        {
            try
            {
                dbManager.OpenConnection();
                string query = $"INSERT INTO employees (LastName, FirstName, Position, DepartmentID) VALUES ('{last}','{first}', '{position}', '{idd}')";
                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                cmd.ExecuteNonQuery();
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
        public DataTable GetDepartments()
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

            return dataTable;
        }
        public AssetType GetAssetTypeById(string id)
        {
            AssetType assetType = null;
            string query = $"SELECT AssetTypeName, Description FROM assettypes WHERE AssetTypeID = {id}";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string name = reader["AssetTypeName"].ToString();
                        string description = reader["Description"].ToString();
                        assetType = new AssetType { Name = name, Description = description };
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

            return assetType;
        }
        public Department GetDepartmentById(string id)
        {
            Department department = null;
            string query = $"SELECT DepartmentName, Description FROM departments WHERE DepartmentID = {id}";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string name = reader["DepartmentName"].ToString();
                        string description = reader["Description"].ToString();
                        department = new Department { Name = name, Description = description };
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

            return department;
        }
        public Employee GetEmployeeById(string id)
        {
            Employee employee = null;
            string query = $"SELECT LastName, FirstName, Position, DepartmentID FROM employees WHERE EmployeeID = {id}";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string last = reader["LastName"].ToString();
                        string first = reader["FirstName"].ToString();
                        string position = reader["Position"].ToString();
                        int idd = reader.GetInt32(reader.GetOrdinal("DepartmentID"));
                        employee = new Employee { LastName = last, FirstName = first, Position = position, DepartmentID = idd };
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

            return employee;
        }
        public bool UpdateAssetType(string id, string newName, string newDescription)
        {
            string query = $"UPDATE assettypes SET AssetTypeName = '{newName}', Description = '{newDescription}' WHERE AssetTypeID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
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
        public bool UpdateDepartment(string id, string newName, string newDescription)
        {
            string query = $"UPDATE departments SET DepartmentName = '{newName}', Description = '{newDescription}' WHERE DepartmentID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
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
        public bool UpdateEmployee(string id, string last, string first, string position, int idd)
        {
            string query = $"UPDATE employees SET LastName = '{last}', Firstname = '{first}', Position = '{position}', DepartmentID = '{idd}' WHERE EmployeeID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
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
        public bool DeleteAssetType(string id)
        {
            string query = $"DELETE FROM assettypes WHERE AssetTypeID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
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
        public bool DeleteDepartment(string id)
        {
            string query = $"DELETE FROM departments WHERE DepartmentID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
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
        public bool DeleteEmployee(string id)
        {
            string query = $"DELETE FROM employees WHERE EmployeeID = '{id}'";

            try
            {
                if (dbManager.OpenConnection())
                {
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
    }
    
}
