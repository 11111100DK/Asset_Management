using AsserManagement.DAO;
using AsserManagement.DTO;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsserManagement.BUS
{
    public class BUSRepair
    {
        private DAORepair dAORepair;

        public BUSRepair()
        {
            dAORepair = new DAORepair();
        }
        public DataTable GetAllRepair(DateTime FromDate, DateTime ToDate)
        {
            return dAORepair.GetAllRepair(FromDate,ToDate);
        }
        public List<KeyValuePair<int, string>> GetAssetKeyValuePairList(string searchKeyword, int assetTypeID, int departmentID, string status)
        {
            return dAORepair.GetAssetKeyValuePairList(searchKeyword, assetTypeID, departmentID, status);
        }
        public List<KeyValuePair<int, string>> GetTypeKeyValuePairList()
        {
            return dAORepair.GetTypeKeyValuePairList();
        }
        public List<KeyValuePair<int, string>> GetDepartmentKeyValuePairList()
        {
            return dAORepair.GetDepartmentKeyValuePairList();
        }
        public List<KeyValuePair<int, string>> GetEmployeeKeyValuePairList()
        {
            return dAORepair.GetEmployeeKeyValuePairList();
        }
        public bool AddRepair(int fixedAssetID, DateTime repairDate, string description, decimal repairCost, int departmentID, int employeeID, string status, DateTime nextRepairDate)
        {
            // Thêm kiểm tra dữ liệu nếu cần
            return dAORepair.AddRepair(fixedAssetID, repairDate, description, repairCost, departmentID, employeeID, status, nextRepairDate);
        }
        public RepairsAndMaintenance GetRepairsAndMaintenanceById(string id)
        {
            return dAORepair.GetRepairsAndMaintenanceById(id);
        }
        public bool UpdateRepairsAndMaintenance(string id, int fixedAssetID, DateTime repairDate, string description, decimal repairCost, int departmentID, int employeeID, string status, DateTime nextRepairDate)
        {
            return dAORepair.UpdateRepairsAndMaintenance(id,fixedAssetID, repairDate, description, repairCost, departmentID, employeeID, status, nextRepairDate);
        }
        public bool DeleteRepairsAndMaintenance(string id)
        {
            return dAORepair.DeleteRepairsAndMaintenance(id);
        }
        public void ExportToExcel(DataTable dataTable)
        {
            dAORepair.ExportToExcel(dataTable);
        }
    }
}
