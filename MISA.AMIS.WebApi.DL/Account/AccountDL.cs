using Dapper;
using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public class AccountDL : BaseDL<Account>, IAccountDL
    {
        private readonly List<Guid> listRecordAffectd = new();
        public AccountDL(IUnitOfWork uow) : base(uow)
        {
        }

        public async Task<bool> CheckAccountNumberExisted(string accountNumber)
        {
            var sql = "select * from \"Account\" where  \"AccountNumber\" = @accountNumber";
            var param = new DynamicParameters();
            param.Add("accountNumber", accountNumber);
            var entity = await Uow.Connection.QuerySingleOrDefaultAsync<Account>(sql, param);
            return entity == null;
        }

        public async Task<int> GetNumberOfChildAsync(Account account)
        {
            var sql = "select count(*) from \"Account\" where \"GeneralAccountId\" = @id";
            var param = new DynamicParameters();
            param.Add("id", account.AccountId);
            var result = await Uow.Connection.QueryFirstOrDefaultAsync<int>(sql, param);
            return result;
        }

        public async Task<int> UpdateActiveAsync(Guid id, bool val)
        {
            var sql = "update \"Account\" set \"Active\" = @active where \"AccountId\" = @id";
            var param = new DynamicParameters();
            param.Add("active", val);
            param.Add("id", id);
            var result = await Uow.Connection.ExecuteAsync(sql, param);
            return result;
        }

        public async Task UpdateActiveRecursiveAsync(Guid id, bool val)
        {
            listRecordAffectd.Add(id);
            var sql = "update \"Account\" set \"Active\" = @active where \"AccountId\" = @id";
            var param = new DynamicParameters();
            param.Add("active", val);
            param.Add("id", id);
            var result = await Uow.Connection.ExecuteAsync(sql, param);
            await UpdateAllChildActiveAsync(id, val);
        }

        public async Task<List<Guid>> UpdateAllChildActiveAsync(Guid parentId, bool val)
        {
            var sql = "select * from \"Account\" where \"GeneralAccountId\" = @parentId";
            var param = new DynamicParameters();
            param.Add("parentId", parentId);
            var children = await Uow.Connection.QueryAsync<Account>(sql, param);
            foreach (var acc in children)
            {
                await UpdateActiveRecursiveAsync(acc.AccountId, val);
            }
            return listRecordAffectd;
        }

        public async Task<int> UpdateIsParentAsync(Account parent, bool val)
        {
            var param = new DynamicParameters();
            param.Add("parentId", parent.AccountId);
            var value = val ? "true" : "false";
            var sql = $"update  \"Account\" set  \"IsParent\" = {value} where \"AccountId\" = @parentId";
            var result = await Uow.Connection.ExecuteAsync(sql, param);
            return result;
        }
    }
}
