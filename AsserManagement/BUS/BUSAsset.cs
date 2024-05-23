using AsserManagement.DAO;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Data;
using System.Windows.Forms;

namespace AsserManagement
{
    public class BUSAsset
    {
        private DAOAsset assetDataAccess;


        public BUSAsset()
        {
            assetDataAccess = new DAOAsset();
        }
        public DataTable GetAllRepair(decimal id)
        {
            return assetDataAccess.GetAllRepair(id);
        }
        public DataTable GetAllTransfer(decimal id)
        {
            return assetDataAccess.GetAllTransfer(id);
        }
        public DataTable GetAllDisposal(decimal id)
        {
            return assetDataAccess.GetAllDisposal(id);
        }
        public DataTable GetAssetDataForPage(int pageNumber, int pageSize)
        {
            return assetDataAccess.GetAssetDataForPage(pageNumber, pageSize);
        }

        public int GetTotalPages(int pageSize, string searchKeyword, int assetTypeID, int departmentID, int employeeID, string status, DateTime FromDate, DateTime ToDate, DateTime FromWarranty, DateTime ToWarranty)
        {
            return assetDataAccess.GetTotalPages(pageSize,searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);
        }
        public int DeleteAsset(int assetID)
        {
            return assetDataAccess.DeleteAsset(assetID);
        }

        public DataTable GetAssetData(string searchKeyword, int assetTypeID, int departmentID, int employeeID, string status, DateTime FromDate, DateTime ToDate, DateTime FromWarranty, DateTime ToWarranty)
        {
            return assetDataAccess.GetAssetData(searchKeyword, assetTypeID, departmentID, employeeID, status, FromDate, ToDate, FromWarranty, ToWarranty);

        }

    }
}
