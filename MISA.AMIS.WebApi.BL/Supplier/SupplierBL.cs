using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.BL
{
    public class SupplierBL : BaseBL<Supplier>, ISupplierBL
    {
        public SupplierBL(ISupplierDL supplierDL) : base(supplierDL)
        {
        }
    }
}
