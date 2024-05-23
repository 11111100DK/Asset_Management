using AsserManagement.DAO;
using AsserManagement.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement.BUS
{
    internal class BUSDisposal
    {
        private DAODisposal dAODisposal;

        public BUSDisposal()
        {
            dAODisposal = new DAODisposal();
        }
        public DataTable GetAllDisposal(DateTime FromDate, DateTime ToDate)
        {
            return dAODisposal.GetAllDisposal(FromDate,ToDate);
        }
        public List<KeyValuePair<int, string>> GetAssetKeyValuePairList(string searchKeyword, int assetTypeID, int departmentID, string status)
        {
            return dAODisposal.GetAssetKeyValuePairList(searchKeyword, assetTypeID, departmentID, status);
        }
        public List<KeyValuePair<int, string>> GetTypeKeyValuePairList()
        {
            return dAODisposal.GetTypeKeyValuePairList();
        }
        public List<KeyValuePair<int, string>> GetDepartmentKeyValuePairList()
        {
            return dAODisposal.GetDepartmentKeyValuePairList();
        }
        public List<KeyValuePair<int, string>> GetEmployeeKeyValuePairList()
        {
            return dAODisposal.GetEmployeeKeyValuePairList();
        }
        public bool AddDisposal(int fixedAssetID, DateTime disposalDate, string reason, decimal saleValue, int departmentID, int employeeID)
        {
            // Gọi phương thức từ lớp DAODisposal để thêm bản ghi
            return dAODisposal.AddDisposal(fixedAssetID, disposalDate, reason, saleValue, departmentID, employeeID);
        }
        public Disposal GetDisposalById(string id)
        {          
                return dAODisposal.GetDisposalById(id);
        }
        public bool UpdateDisposal(string id, int fixedAssetID, DateTime disposalDate, string reason, decimal saleValue, int departmentID, int employeeID)
        {
            return dAODisposal.UpdateDisposal(id, fixedAssetID, disposalDate, reason,  saleValue, departmentID, employeeID);
        }
        public bool DeleteDisposal(string id)
        {
            return dAODisposal.DeleteDisposal(id);
        }
        public void ExportToExcel(DataTable dataTable)
        {
            dAODisposal.ExportToExcel(dataTable);
        }
    }
}
