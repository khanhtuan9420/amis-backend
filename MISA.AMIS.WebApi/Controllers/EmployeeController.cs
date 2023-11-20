using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.WebApi.BL;
using MISA.AMIS.WebApi.Common;

namespace MISA.AMIS.WebApi.Controllers
{
    public class EmployeeController : BaseController<Employee>
    {
        public EmployeeController(IEmployeeBL employeeBL) : base(employeeBL)
        {
        }
    }
}
