using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement.DTO
{
    public class RepairsAndMaintenance
    {
        public int RepairID { get; set; }
        public int FixedAssetID { get; set; }
        public DateTime RepairDate { get; set; }
        public string Description { get; set; }
        public decimal RepairCost { get; set; }
        public int DepartmentID { get; set; }
        public int EmployeeID { get; set; }
        public string Status { get; set; }
        public DateTime NextRepairDate { get; set; }

        public RepairsAndMaintenance()
        {
            // Constructor mặc định không tham số
        }

        public RepairsAndMaintenance(int repairID, int fixedAssetID, DateTime repairDate, string description, decimal repairCost, int departmentID, int employeeID, string status, DateTime nextRepairDate)
        {
            RepairID = repairID;
            FixedAssetID = fixedAssetID;
            RepairDate = repairDate;
            Description = description;
            RepairCost = repairCost;
            DepartmentID = departmentID;
            EmployeeID = employeeID;
            Status = status;
            NextRepairDate = nextRepairDate;
        }
    }
}
