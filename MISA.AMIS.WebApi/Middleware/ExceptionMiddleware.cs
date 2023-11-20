using Microsoft.Extensions.Localization;
using System.Resources;
using System.Globalization;
using System.Reflection;
using MISA.AMIS.MF1732.Domain;
using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.Common.Enum;

namespace MISA.AMIS.WebApi
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Xửa lý ngoại lệ
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="ex">Ngoại lệ</param>
        /// <returns></returns>
        /// Created by: dktuan (17/09/2023)
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            Console.WriteLine(ex);
            context.Response.ContentType = "application/json";
            if (ex is NotFoundException notFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(text: new BaseException
                {
                    ErrorCode = notFoundException.ErrorCode,
                    UserMessage = Resources.NotFoundResource,
                    DevMessage =  Resources.NotFoundResource,
                    TraceId = context.TraceIdentifier,
                    MoreInfo = ex.HelpLink,
                }.ToString()
                );
            }
            else if (ex is ConflictException conflictException)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                var exception = conflictException.Exception;
                await context.Response.WriteAsync(text: new BaseException
                {
                    ErrorCode = exception.ErrorCode,
                    UserMessage = exception.UserMessage??Resources.ConflictResource,
                    DevMessage = exception.DevMessage??ex.Message,
                    TraceId = context.TraceIdentifier,
                    MoreInfo = ex.HelpLink,
                    OtherData = exception.OtherData,
                    Errors = exception.Errors
                }.ToString()
                );
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(text: new BaseException
                {
                    ErrorCode = ErrorCode.ServerError,
                    UserMessage = Resources.ServerError,
#if DEBUG
                    DevMessage = ex.Message,
#else
                    DevMessage = "",
#endif
                    TraceId = context.TraceIdentifier,
                    MoreInfo = ex.HelpLink,
                }.ToString()
                );
            }
        }
    }
}
