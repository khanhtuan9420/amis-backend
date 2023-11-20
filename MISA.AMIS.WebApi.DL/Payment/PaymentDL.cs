using Dapper;
using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public class PaymentDL : BaseDL<Payment>, IPaymentDL
    {
        public PaymentDL(IUnitOfWork uow) : base(uow)
        {
            ListPropsExcluded = new List<string> {"SupplierCode" };
        }

        public async Task<string> GetNewCode(IUnitOfWork? uow=null)
        {
            if (uow != null) Uow = uow;
            var sql = "select \"PaymentNo\" from \"Payment\" order by \"CreatedDate\" desc limit 1";
            var code = await Uow.Connection.QuerySingleOrDefaultAsync<string>(sql);
            var newCode = IncrementString(code);
            return newCode;
        }

        public override async Task<Payment> GetRecordByIdAsync(Guid id, IUnitOfWork? uow)
        {
            if (uow != null) Uow = uow;
            var connection = await Uow.OpenConnectionAsync();
            var sql = $"select * from {TableName.ToLower()}_view WHERE \"{TableName}Id\" = @id";
            var param = new DynamicParameters();
            param.Add("id", id);
            var result = await connection.QuerySingleOrDefaultAsync<Payment>(sql, param);
            return result;
        }

        static string IncrementString(string input)
        {
            string pattern = @"^([a-zA-Z]+)(\d+)$";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                string prefix = match.Groups[1].Value;
                int number = int.Parse(match.Groups[2].Value);
                number++;

                return $"{prefix}{number:D3}";
            }
            else
            {
                return input + "1";
            }
        }

        public async Task<int> GetTotalAsync(IUnitOfWork? uow)
        {
            if (uow != null) Uow = uow;
            var sql = "select sum(\"Total\") from \"Payment\"";
            var result = await Uow.Connection.QueryFirstOrDefaultAsync<int>(sql);
            return result;
        }

        public override async Task<bool> CheckConflictCode(string code, IUnitOfWork? uow = null)
        {
            if(uow != null) Uow = uow;
            var sql = $"select * from \"Payment\" where \"PaymentNo\"=@code";
            var param = new DynamicParameters();
            param.Add("code", code);
            var payment = await Uow.Connection.QueryFirstOrDefaultAsync<Payment>(sql, param);
            return payment == null;
        }
    }
}
