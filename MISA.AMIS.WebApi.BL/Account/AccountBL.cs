using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.Common.Enum;
using MISA.AMIS.WebApi.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.BL
{
    public class AccountBL : BaseBL<Account>, IAccountBL
    {
        private readonly IAccountDL _accountDL;
        public AccountBL(IAccountDL accountDL) : base(accountDL)
        {
            _accountDL = accountDL;
        }

        /// <summary>
        /// Validate giá trị bản ghi trước khi tạo
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        /// Created by: dktuan(07/11/2023)
        public override async Task ExcuteCreateBusiness(Account account)
        {
            var check = await _accountDL.CheckAccountNumberExisted(account.AccountNumber);
            if (!check)
            {
                throw new ConflictException
                {
                    ErrorCode = ErrorCode.CodeExisted,
                };
            }
            var id = account.GeneralAccountId ?? Guid.Empty;
            account.Active = true;
            if (id == Guid.Empty)
            {
                account.Grade = 1;
            }
            var parent = await _accountDL.GetRecordByIdAsync(id);
            if (parent != null)
            {
                account.Grade = parent.Grade + 1;
                await _accountDL.UpdateIsParentAsync(parent, true);
            }
        }

        public override async Task ExecuteUpdateBusiness(Guid id, Account account)
        {
            var oldAccount = await _accountDL.GetRecordByIdAsync(id);
            if (oldAccount != null && oldAccount.GeneralAccountId != account.GeneralAccountId)
            {
                var oldParent = await _accountDL.GetRecordByIdAsync((Guid)oldAccount.GeneralAccountId);
                var childrenNum = await _accountDL.GetNumberOfChildAsync(oldParent);
                if (childrenNum == 1)
                {
                    await _accountDL.UpdateIsParentAsync(oldParent, false);
                }
                if (account.GeneralAccountId != Guid.Empty)
                {
                    var newGenAccount = await _accountDL.GetRecordByIdAsync((Guid)account.GeneralAccountId);
                    await _accountDL.UpdateIsParentAsync(newGenAccount, true);
                    account.Grade = newGenAccount.Grade + 1;
                }
            }
        }

        public override async Task ExecuteDeleteBusiness(Guid id)
        {
            var entity = await _accountDL.GetRecordByIdAsync((Guid)id);
            if (entity != null)
            {
                var parent = await _accountDL.GetRecordByIdAsync((Guid)entity.GeneralAccountId);
                if (parent != null)
                {
                    var num = await _accountDL.GetNumberOfChildAsync(parent);
                    if (num<2) await _accountDL.UpdateIsParentAsync(parent, false);

                }
            }
        }

        public async Task<List<Guid>> UpdateActiveAsync(Guid id, bool val, bool forceChild)
        {
            var result = new List<Guid>();
            await _accountDL.UpdateActiveAsync(id, val);
            if(!val || (val && forceChild))
            {
                result = await _accountDL.UpdateAllChildActiveAsync(id, val);
            }
            result.Add(id);
            return result;
        }
    }
}
