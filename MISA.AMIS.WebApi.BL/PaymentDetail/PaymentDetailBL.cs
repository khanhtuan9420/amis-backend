using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.BL
{
    public class PaymentDetailBL : BaseBL<PaymentDetail>, IPaymentDetailBL
    {
        private readonly IPaymentDetailDL _paymentDetailDL;
        public PaymentDetailBL(IPaymentDetailDL paymentDetailDL) : base(paymentDetailDL)
        {
            _paymentDetailDL = paymentDetailDL;
        }

        public async Task<object> GetDetailsByMasterId(Guid id, string? filter, int? pageSize, int? pageNum)
        {
            var result = await _paymentDetailDL.GetDetailByMasterId(id, filter, pageSize, pageNum);
            var totalRecords = await _paymentDetailDL.GetTotalRecordsAsync(id);
            return new
            {
                Data = result,
                TotalRecord = totalRecords
            };
        }
    }
}
