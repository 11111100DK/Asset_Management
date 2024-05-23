using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement.DTO
{
    public class HistoryTransferAsset
    {
        public int HistoryTransferAssetID { get; set; }
        public int FixedAssetID { get; set; }
        public DateTime TransferDate { get; set; }
        public int FromDepartmentID { get; set; }
        public int ToDepartmentID { get; set; }
        public string TransferReason { get; set; }
        public string Notes { get; set; }
        public int FromEmployeeID { get; set; }
        public int ToEmployeeID { get; set; }
    }
}
