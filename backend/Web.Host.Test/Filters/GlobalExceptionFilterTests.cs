using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Web.Host.Filters;
using Web.Host.Models.Responses;

namespace Web.Host.Test.Filters;

public class GlobalExceptionFilterTests
{
    #region Private Fields
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly GlobalExceptionFilter _sut;
    #endregion

    #region Constructor
    public GlobalExceptionFilterTests()
    {
        _logger = Substitute.For<ILogger<GlobalExceptionFilter>>();
        _sut = new GlobalExceptionFilter(_logger);
    }
    #endregion

    #region Private Methods
    private static ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionDescriptor = new ActionDescriptor
        {
            RouteValues = new Dictionary<string, string?>
            {
                ["controller"] = "TestController",
                ["action"] = "TestAction"
            }
        };
        var actionContext = new ActionContext(httpContext, new RouteData(), actionDescriptor);
        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    private static void AssertResult(ExceptionContext context, int expectedStatusCode, string expectedMessage)
    {
        Assert.True(context.ExceptionHandled);
        var jsonResult = Assert.IsType<JsonResult>(context.Result);
        Assert.Equal(expectedStatusCode, jsonResult.StatusCode);
        var response = Assert.IsType<ClientResponse<object>>(jsonResult.Value);
        Assert.False(response.Success);
        Assert.Null(response.Body);
        Assert.NotNull(response.Error);
        Assert.Equal(expectedStatusCode, response.Error.StatusCode);
        Assert.Equal(expectedMessage, response.Error.Message);
    }

    public static IEnumerable<object[]> ExceptionResults => new List<object[]>
    {
        new object[] { new FileNotFoundException("File missing"), StatusCodes.Status404NotFound, "File missing" },
        new object[] { new InvalidOperationException("Invalid operation"), StatusCodes.Status400BadRequest, "Invalid operation" },
        new object[] { new BadHttpRequestException("Bad request"), StatusCodes.Status400BadRequest, "Bad request" },
        new object[] { new Exception("Something went wrong"), StatusCodes.Status500InternalServerError, "An unexpected error occurred." }
    };
    #endregion


    [Theory]
    [MemberData(nameof(ExceptionResults))]
    public void OnException_MapsExceptionToResponse(Exception exception, int expectedStatusCode, string expectedMessage)
    {
        var context = CreateExceptionContext(exception);

        _sut.OnException(context);

        AssertResult(context, expectedStatusCode, expectedMessage);
    }

}

