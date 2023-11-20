using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class UpdateActiveStatus
    {
        public Guid Id { get; set; }
        public bool Value { get; set; }
        public bool ForceChild { get; set; }
    }
}
