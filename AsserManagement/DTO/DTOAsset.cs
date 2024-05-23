using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement
{
    internal class DTOAsset
    {
        public int FixedAssetID { get; set; }
        public string AssetName { get; set; }
        public int AssetTypeID { get; set; }
        public int SupplierID { get; set; }
        public int DepartmentID { get; set; }
        public int? EmployeeID { get; set; }
        public decimal Value { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; }
    }

}
