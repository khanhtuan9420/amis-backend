using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.WebApi
{
    public class ConflictException : Exception
    {
        public ErrorCode ErrorCode { get; set; }
        public BaseException Exception { get; set; }
        public ConflictException() { }
        public ConflictException(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string message, ErrorCode errorCode, BaseException ex) : base(message)
        {
            ErrorCode = errorCode;
            Exception = ex;
        }
    }
}
