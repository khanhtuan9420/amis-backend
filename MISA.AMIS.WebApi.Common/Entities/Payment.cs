using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class Payment : BaseEntity, IBaseEntity
    {
        public Guid PaymentId { get; set; }
        [Required(ErrorMessage = "Số chứng từ không được để trống")]
        public string PaymentNo { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime RecordDate { get; set; }
        public int? Total { get; set; } = 0;
        public Guid? SupplierId { get; set; }

        public string? SupplierCode { get; set; }
        public string? SupplierContactName { get; set; }

        public string? Address { get; set; }
        public string? ReceiveName { get; set; }
        public string Explain { get; set; }
        public int? DocumentInclude { get; set; }
        public Guid? EmployeeId { get; set; }

        public Guid GetId()
        {
            return PaymentId;
        }

        public void SetId(Guid id)
        {
            PaymentId = id;
        }
    }
}
