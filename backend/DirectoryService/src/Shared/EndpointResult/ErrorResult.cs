using Microsoft.AspNetCore.Http;
using Shared.ResultPattern;

namespace Shared.EndpointResult
{
    public sealed class ErrorResult : IResult
    {
        private readonly Error _error;

        public ErrorResult(Error error)
        {
            _error = error;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            var envelope = Envelope.Fail(_error);

            httpContext.Response.StatusCode = GetStatusCodeFromErrorType(_error.Type);

            return httpContext.Response.WriteAsJsonAsync(envelope);
        }

        private static int GetStatusCodeFromErrorType(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
                ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
                ErrorType.CONFLICT => StatusCodes.Status409Conflict,
                ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
