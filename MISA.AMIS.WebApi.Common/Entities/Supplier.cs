using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class Supplier : BaseEntity, IBaseEntity
    {
        public Guid SupplierId { get; set; }
        public string SupplierCode {  get; set; }
        public string SupplierName { get; set;}

        public string TaxCode { get; set; }

        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public Guid GetId()
        {
            return SupplierId;
        }

        public void SetId(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
