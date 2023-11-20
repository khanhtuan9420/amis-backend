using MISA.AMIS.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.BL
{
    public interface IPaymentBL : IBaseBL<Payment>
    {
        public Task<string> GetNewCode();
        public Task<dynamic> GetPaymentsAsync(string? keyWord, int? pageSize, int? pageNumber);
        public Task<Payment> CreateRecordAsync(Payment payment, List<PaymentDetail> records);

        public Task<MemoryStream> ExportAsync(string filters);

        public Task UpdateRecordAsync(Payment payment, IEnumerable<PaymentDetail> paymentDetailsUpdate, IEnumerable<PaymentDetail> paymentDetailsCreate, List<Guid> detailDeleteIds);

        public Task<int> DeleteRecordsAsync(List<Guid> ids);
    }
}
