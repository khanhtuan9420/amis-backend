using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common.Enum
{
    public enum ErrorCode
    {
        /// <summary>
        /// Mã code đã tồn tại
        /// </summary>
        CodeExisted = 1,
        /// <summary>
        /// Không tìm thấy
        /// </summary>
        NotFound = 2,
        /// <summary>
        /// Lỗi nhập liệu
        /// </summary>
        InputError = 3,
        /// <summary>
        /// Lỗi server
        /// </summary>
        ServerError = 4,
    }
}
