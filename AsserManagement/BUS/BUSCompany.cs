using AsserManagement.DAO;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsserManagement.BUS
{
    public class BUSCompany
    {
        private DAOCompany dAOCompany;
        public BUSCompany()
        {
            dAOCompany = new DAOCompany();
        }
        public DataTable GetAllSuppliers()
        {
            return dAOCompany.GetAllSuppliers();
        }
        public bool AddSupplier(string name, string address, string contact)
        {
            // Thêm kiểm tra dữ liệu nếu cần
            return dAOCompany.AddSupplier(name, address, contact);
        }
        public Supplier GetSupplierById(string id)
        {
            return dAOCompany.GetSupplierById(id);
        }
        public bool UpdateSupplier(string id, string newName, string newAddress, string newContact)
        {
            return dAOCompany.UpdateSupplier(id, newName, newAddress, newContact);
        }
        public bool DeleteSupplier(string id)
        {
            return dAOCompany.DeleteSupplier(id);
        }
    }
}
