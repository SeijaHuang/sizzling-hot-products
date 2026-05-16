using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Host.Models.Responses;

namespace Web.Host.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{

    public void OnException(ExceptionContext context)
    {
        var (statusCode, message) = context.Exception switch
        {
            FileNotFoundException => (StatusCodes.Status404NotFound, context.Exception.Message),
            InvalidOperationException => (StatusCodes.Status400BadRequest, context.Exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        var response = new ClientResponse<object>
        {
            Success = false,
            Error = new ClientError
            {
                StatusCode = statusCode,
                Message = message
            },
            Body = null
        };

        context.Result = new JsonResult(response)
        {
            StatusCode = statusCode
        };
        context.ExceptionHandled = true;
    }
}

