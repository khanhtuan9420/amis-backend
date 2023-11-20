using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class PaymentDetail : BaseEntity, IBaseEntity
    {
        public Guid? PaymentDetailId { get; set; }
        public Guid? PaymentId { get; set; }
        [Required(ErrorMessage ="Tài khoản nợ không được để trống")]
        public Guid DebitAccountId { get; set; }
        [Required(ErrorMessage = "Tài khoản có không được để trống")]
        public Guid CreditAccountId { get; set; }
        public int? Amount { get; set; } = 0;
        public Guid? SupplierId { get; set; }
        public string? CreditAccountNumber { get; set; }
        public string? DebitAccountNumber { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }

        public string? Explain {  get; set; }

        public Guid GetId()
        {
            return PaymentDetailId??Guid.Empty;
        }

        public void SetId(Guid id)
        {
            PaymentDetailId = id;
        }
    }
}
