using MISA.AMIS.WebApi.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi.Common
{
    public class BaseException
    {
        #region Properties
        /// <summary>
        /// Mã lỗi
        /// </summary>
        public ErrorCode ErrorCode { get; set; }
        /// <summary>
        /// Mã thông báo cho Dev
        /// </summary>
        public string? DevMessage { get; set; }
        /// <summary>
        /// Mã thông báo cho người dùng
        /// </summary>
        public string? UserMessage { get; set; }
        /// <summary>
        /// traceId
        /// </summary>
        public string? TraceId { get; set; }
        /// <summary>
        /// Thông tin thêm
        /// </summary>
        public string? MoreInfo { get; set; }
        /// <summary>
        /// Danh sách lỗi
        /// </summary>
        public object? Errors { get; set; }

        public object? OtherData { get; set; }
        #endregion

        #region Method
        /// <summary>
        /// Chuyển sang Json
        /// </summary>
        /// <returns>json</returns>
        /// Created by: dktuan (17/09/2023)
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
        #endregion
    }
}
