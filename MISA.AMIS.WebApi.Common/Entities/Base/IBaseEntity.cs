using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public interface IBaseEntity
    {
        public Guid GetId();
        public void SetId(Guid id);
    }
}
