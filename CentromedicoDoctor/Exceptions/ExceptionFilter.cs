using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Exceptions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var _exception = context.Exception;

            if (context.Exception is AggregateException)
                _exception = context.Exception.InnerException;

            var statusCode = HttpStatusCode.InternalServerError;
            var customError = false;
            if (context.Exception is EntityNotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }


            switch (_exception)
            {

                case BadHttpRequestException or BadHttpRequestException or ArgumentOutOfRangeException or ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    customError = true;
                    break;

                case UnauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    customError = true;
                    break;

                case NoContentException:
                    statusCode = HttpStatusCode.NoContent;
                    customError = true;
                    break;

            }


            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.Result = new JsonResult(new
            {
                error = new[] { _exception.Message },
                Exception = _exception,
                customError = customError
            });
        }
    }
}
