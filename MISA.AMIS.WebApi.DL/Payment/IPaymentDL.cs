using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public interface IPaymentDL : IBaseDL<Payment>
    {
        public Task<string> GetNewCode(IUnitOfWork? uow=null);
        public Task<int> GetTotalAsync(IUnitOfWork? uow = null);
    }
}
