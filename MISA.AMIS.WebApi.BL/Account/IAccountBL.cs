using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.BL
{
    public interface IAccountBL : IBaseBL<Account>
    {
        public Task<List<Guid>> UpdateActiveAsync(Guid id, bool val, bool forceChild);
    }
}
