using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement
{
    internal class DatabaseManager
    {
        private MySqlConnection connection; 
        private string server;
        private string database;
        private string username;
        private string password;

        public DatabaseManager()
        {
            Initialize();
            CheckAndUpdateAssetStatus();
        }

        // Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "asset_management";
            username = "root";
            password = "";
            string connectionString = $"Server={server};Database={database};Uid={username};Pwd={password};";
            connection = new MySqlConnection(connectionString);
        }

        // Open connection to the database
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                // Handle exception
                return false;
            }
        }

        // Close connection
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                // Handle exception
                return false;
            }
        }
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        // Execute query
        public MySqlDataReader ExecuteQuery(string query)
        {
            MySqlCommand cmd = new MySqlCommand(query, connection);
            return cmd.ExecuteReader();
        }

        public void CheckAndUpdateAssetStatus()
        {
            try
            {
                // Mở kết nối đến cơ sở dữ liệu
                if (OpenConnection())
                {
                    // Lấy ngày hiện tại
                    DateTime currentDate = DateTime.Now.Date;
                    DateTime futureDate = currentDate.AddDays(14);

                    // Tạo truy vấn để cập nhật trạng thái
                    string updateQuery = @"
                UPDATE fixedassets 
                SET Status = 'Cần bảo trì' 
                WHERE WarrantyDate <= @FutureDate AND Status = 'Đang sử dụng' AND WarrantyDate IS NOT NULL";

                    // Thực thi truy vấn cập nhật
                    MySqlCommand cmd = new MySqlCommand(updateQuery, connection);
                    cmd.Parameters.AddWithValue("@FutureDate", futureDate);
                    cmd.ExecuteNonQuery();

                    // Đóng kết nối
                    CloseConnection();
                }
                else
                {
                    // Xử lý trường hợp không thể mở kết nối
                    Console.WriteLine("Không thể kết nối.");
                }
            }
            catch (MySqlException ex)
            {
                // Xử lý ngoại lệ
                Console.WriteLine("Lỗi kết nối: " + ex.Message);
            }
        }


    }
}
