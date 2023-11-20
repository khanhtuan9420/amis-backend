using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.WebApi.BL;
using MISA.AMIS.WebApi.Common;

namespace MISA.AMIS.WebApi.Controllers
{
    public class SupplierController : BaseController<Supplier>
    {
        public SupplierController(ISupplierBL supplierBL) : base(supplierBL)
        {
        }
    }
}
