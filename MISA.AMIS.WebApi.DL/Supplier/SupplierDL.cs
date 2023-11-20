using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public class SupplierDL : BaseDL<Supplier>, ISupplierDL
    {
        public SupplierDL(IUnitOfWork uow) : base(uow)
        {
        }
    }
}
