using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsserManagement.DAO
{
    public class DAOCompany
    {
        private DatabaseManager dbManager;
        public DAOCompany()
        {
            dbManager = new DatabaseManager();
        }
        public DataTable GetAllSuppliers()
        {
            DataTable dataTable = new DataTable();

            try
            {
                dbManager.OpenConnection();
                string query = "SELECT SupplierID as 'ID', SupplierName as 'Name', Address as 'Address', ContactInformation as 'Contact' FROM suppliers"; // Điều chỉnh tên cột ở đây
                MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dataTable);

            }
            catch (Exception ex)
            {
            }
            finally
            {
                dbManager.CloseConnection();
            }

            return dataTable;
        }
        public bool AddSupplier(string name, string address, string contact)
        {
            try
            {
                dbManager.OpenConnection();
                string query = $"INSERT INTO suppliers (SupplierName, Address, ContactInformation) VALUES ('{name}', '{address}', '{contact}')";
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
        public Supplier GetSupplierById(string id)
        {
            Supplier supplier = null;
            string query = $"SELECT SupplierName, Address, ContactInformation FROM suppliers WHERE SupplierID = {id}";

            try
            {
                if (dbManager.OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, dbManager.Connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string name = reader["SupplierName"].ToString();
                        string address = reader["Address"].ToString();
                        string contact = reader["ContactInformation"].ToString();

                        supplier = new Supplier { Name = name, Address = address, ContactInformation = contact };
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

            return supplier;
        }
        public bool UpdateSupplier(string id, string newName, string newAddress, string newContact)
        {
            string query = $"UPDATE suppliers SET SupplierName = '{newName}', Address = '{newAddress}', ContactInformation = '{newContact}' WHERE SupplierID = '{id}'";

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
        public bool DeleteSupplier(string id)
        {
            string query = $"DELETE FROM suppliers WHERE SupplierID = '{id}'";

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
