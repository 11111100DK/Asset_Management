using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement
{
    public class BUSInfo
    {
        private DAOInfo dAOInfo;

        public BUSInfo()
        {
            dAOInfo = new DAOInfo();
        }

        public DataTable GetAllAssetTypes()
        {
            return dAOInfo.GetAllAssetTypes();
        }
        public DataTable GetAllDepartments()
        {
            return dAOInfo.GetAllDepartments();
        }
        public DataTable GetAllEmployees()
        {
            return dAOInfo.GetAllEmployees();
        }
        public bool AddAssetType(string name, string description)
        {
            // Thêm kiểm tra dữ liệu nếu cần
            return dAOInfo.AddAssetType(name, description);
        }
        public bool AddDepartment(string name, string description)
        {
            // Thêm kiểm tra dữ liệu nếu cần
            return dAOInfo.AddDepartment(name, description);
        }
        public bool AddEmployee(string last, string first, string position, int idd)
        {
            // Thêm kiểm tra dữ liệu nếu cần
            return dAOInfo.AddEmployee(last, first, position, idd);
        }
        public List<KeyValuePair<int, string>> GetDepartmentList()
        {
            DataTable departments = dAOInfo.GetDepartments();
            List<KeyValuePair<int, string>> departmentList = new List<KeyValuePair<int, string>>();

            foreach (DataRow row in departments.Rows)
            {
                int departmentID = Convert.ToInt32(row["DepartmentID"]);
                string departmentName = row["DepartmentName"].ToString();
                departmentList.Add(new KeyValuePair<int, string>(departmentID, departmentName));
            }

            return departmentList;
        }
        public AssetType GetAssetTypeById(string id)
        {
            return dAOInfo.GetAssetTypeById(id);
        }
        public Department GetDepartmentById(string id)
        {
            return dAOInfo.GetDepartmentById(id);
        }
        public Employee GetEmployeeById(string id)
        {
            return dAOInfo.GetEmployeeById(id);
        }
        public bool UpdateAssetType(string id, string newName, string newDescription)
        {
            return dAOInfo.UpdateAssetType(id, newName, newDescription);
        }
        public bool UpdateDepartment(string id, string newName, string newDescription)
        {
            return dAOInfo.UpdateDepartment(id, newName, newDescription);
        }
        public bool UpdateEmployee(string id, string last, string first, string position, int idd)
        {
            return dAOInfo.UpdateEmployee(id, last, first, position, idd);
        }
        public bool DeleteAssetType(string id)
        {
            return dAOInfo.DeleteAssetType(id);
        }
        public bool DeleteDepartment(string id)
        {
            return dAOInfo.DeleteDepartment(id);
        }
        public bool DeleteEmployee(string id)
        {
            return dAOInfo.DeleteEmployee(id);
        }
    }
}
