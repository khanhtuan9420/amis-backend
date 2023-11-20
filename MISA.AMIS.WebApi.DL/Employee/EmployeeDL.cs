using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public class EmployeeDL : BaseDL<Employee>, IEmployeeDL
    {
        public EmployeeDL(IUnitOfWork uow) : base(uow)
        {
        }
    }
}
