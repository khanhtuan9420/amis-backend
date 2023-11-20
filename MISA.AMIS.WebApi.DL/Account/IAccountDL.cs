using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public interface IAccountDL : IBaseDL<Account>
    {
        public Task<bool> CheckAccountNumberExisted(string accountNumber);
        public Task<int> UpdateIsParentAsync(Account account, bool val);

        public Task<int> GetNumberOfChildAsync(Account account);

        public Task<int> UpdateActiveAsync(Guid id, bool val);

        public Task<List<Guid>> UpdateAllChildActiveAsync(Guid parentId, bool val);
    }
}
