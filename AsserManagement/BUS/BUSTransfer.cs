using AsserManagement.DAO;
using AsserManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsserManagement.BUS
{
    internal class BUSTransfer
    {
        private DAOTransfer dAOTransfer;

        public BUSTransfer()
        {
            dAOTransfer = new DAOTransfer();
        }
        public DataTable GetAllTransfer(DateTime FromDate, DateTime ToDate)
        {
            return dAOTransfer.GetAllTransfer(FromDate, ToDate);
        }
        public List<KeyValuePair<int, string>> GetTypeKeyValuePairList()
        {
            return dAOTransfer.GetTypeKeyValuePairList();
        }
        public List<KeyValuePair<int, string>> GetDepartmentKeyValuePairList()
        {
            return dAOTransfer.GetDepartmentKeyValuePairList();
        }
        public List<KeyValuePair<int, string>> GetEmployeeKeyValuePairList()
        {
            return dAOTransfer.GetEmployeeKeyValuePairList();
        }
        public List<KeyValuePair<int, string>> GetAssetKeyValuePairList(string searchKeyword, int assetTypeID, int departmentID, string status)
        {
            return dAOTransfer.GetAssetKeyValuePairList(searchKeyword, assetTypeID, departmentID, status);
        }
        public int GetDepartmentIDByFixedAssetID(int fixedAssetID)
        {
            return dAOTransfer.GetDepartmentIDByFixedAssetID(fixedAssetID);
        }

        // Phương thức để lấy EmployeeID từ bảng fixedassets dựa trên FixedAssetID
        public int GetEmployeeIDByFixedAssetID(int fixedAssetID)
        {
            return dAOTransfer.GetEmployeeIDByFixedAssetID(fixedAssetID);
        }

        // Phương thức để lấy DepartmentName từ bảng departments dựa trên DepartmentID
        public string GetDepartmentNameByID(int departmentID)
        {
            return dAOTransfer.GetDepartmentNameByID(departmentID);
        }

        // Phương thức để lấy EmployeeName từ bảng employees dựa vào EmployeeID
        public string GetEmployeeNameByID(int employeeID)
        {
            return dAOTransfer.GetEmployeeNameByID(employeeID);
        }
        public bool AddTransfer(int fixedAssetID, DateTime transferDate, int fromDepartmentID, int toDepartmentID, string transferReason, string notes, int fromEmployeeID, int toEmployeeID)
        {
                // Gọi phương thức từ DAO để thêm dữ liệu vào cơ sở dữ liệu
                return dAOTransfer.AddTransfer(fixedAssetID, transferDate, fromDepartmentID, toDepartmentID, transferReason, notes, fromEmployeeID, toEmployeeID);

        }
        public HistoryTransferAsset GetHistoryTransferAssetById(string id)
        {
            return dAOTransfer.GetHistoryTransferAssetById(id);
        }
        public bool UpdateHistoryTransferAsset(string id, int fixedAssetID, DateTime transferDate, int fromDepartmentID, int toDepartmentID, string transferReason, string notes, int fromEmployeeID, int toEmployeeID)
        {
            // Kiểm tra các thông tin bắt buộc không được null hoặc rỗng
            if (string.IsNullOrEmpty(id) || fixedAssetID == 0 || transferDate == null || fromDepartmentID == 0 || toDepartmentID == 0)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return false;
            }

            // Kiểm tra các thông tin không bắt buộc
            if (string.IsNullOrEmpty(transferReason))
            {
                transferReason = string.Empty;
            }

            if (string.IsNullOrEmpty(notes))
            {
                notes = string.Empty;
            }

            try
            {
                // Gọi phương thức từ DAO để cập nhật bản ghi
                return dAOTransfer.UpdateHistoryTransferAsset(id, fixedAssetID, transferDate, fromDepartmentID, toDepartmentID, transferReason, notes, fromEmployeeID, toEmployeeID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
                return false;
            }
        }
        public bool DeleteHistoryTransferAsset(string id, int fromDepartmentID, int fromEmployeeID)
        {
            return dAOTransfer.DeleteHistoryTransferAsset(id, fromDepartmentID, fromEmployeeID);
        }
        public void ExportToExcel(DataTable dataTable)
        {
            dAOTransfer.ExportToExcel(dataTable);
        }
    }
}
