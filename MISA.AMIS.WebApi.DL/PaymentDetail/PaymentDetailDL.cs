using Dapper;
using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public class PaymentDetailDL : BaseDL<PaymentDetail>, IPaymentDetailDL
    {
        public PaymentDetailDL(IUnitOfWork uow) : base(uow)
        {
            ListPropsExcluded = new List<string> { "CreditAccountNumber", "DebitAccountNumber", "SupplierCode", "SupplierName" };
        }

        public async Task<int> DeleteRecordsByMasterIds(List<Guid> ids, IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            var sql = $"delete from \"PaymentDetail\" where \"PaymentId\" = ANY(@idList)";
            var param = new DynamicParameters();
            param.Add("idList", ids);
            var result = await Uow.Connection.ExecuteAsync(sql, param);
            return result;
        }

        public async Task<IEnumerable<PaymentDetail>> GetDetailByMasterId(Guid id, string? filter, int? pageSize, int? pageNum , IUnitOfWork? uow = null)
        {
            if (uow != null) Uow = uow;
            if (string.IsNullOrEmpty(filter)) filter = "";
            int? startRow = (pageNum - 1) * pageSize;
            var sql = $"select * from func_paymentdetail_filter(@filter, @pageSize, @startRow, @masterId)";
            var param = new DynamicParameters();
            param.Add("filter", filter);
            param.Add("pageSize", pageSize);
            param.Add("startRow", startRow);
            param.Add("masterId", id);
            var result = await Uow.Connection.QueryAsync<PaymentDetail>(sql, param);
            return result;
        }

        public async Task<int> GetTotalRecordsAsync(Guid masterId, IUnitOfWork? uow=null)
        {
            if (uow != null) Uow = uow;
            var sql = $"select count(*) from \"PaymentDetail\" where \"PaymentId\" = @id";
            var param = new DynamicParameters();
            param.Add("id", masterId);
            var result = await Uow.Connection.QueryFirstOrDefaultAsync<int>(sql, param);
            return result;
        }
    }
}
