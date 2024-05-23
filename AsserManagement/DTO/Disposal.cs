using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement.DTO
{
    public class Disposal
    {
        public int DisposalID { get; set; }
        public int FixedAssetID { get; set; }
        public DateTime DisposalDate { get; set; }
        public string Reason { get; set; }
        public decimal SaleValue { get; set; }
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
    }
}
