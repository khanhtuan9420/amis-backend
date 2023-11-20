using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class PaymentCreateDto
    {
        public Payment Payment {  get; set; }
        public List<PaymentDetail> Details { get; set; }
    }
}
