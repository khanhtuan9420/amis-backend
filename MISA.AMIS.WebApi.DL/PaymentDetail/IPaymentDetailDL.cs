using Dapper;
using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.DL
{
    public interface IPaymentDetailDL : IBaseDL<PaymentDetail>
    {
        public Task<IEnumerable<PaymentDetail>> GetDetailByMasterId(Guid id, string? filter, int? pageSize, int? pageNum, IUnitOfWork? uow = null);
        public Task<int> DeleteRecordsByMasterIds(List<Guid> ids, IUnitOfWork? uow = null);

        public Task<int> GetTotalRecordsAsync(Guid masterId, IUnitOfWork? uow = null);
    }
}
