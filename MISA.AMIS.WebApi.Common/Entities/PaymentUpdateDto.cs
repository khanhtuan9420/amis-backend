using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class PaymentUpdateDto
    {
        public Payment Payment { get; set; }
        public IEnumerable<PaymentDetail> DetailsUpdate { get; set; }
        public List<Guid> DetailDeleteIds {  get; set; }

        public IEnumerable<PaymentDetail> DetailsCreate {  get; set; }
    }
}
