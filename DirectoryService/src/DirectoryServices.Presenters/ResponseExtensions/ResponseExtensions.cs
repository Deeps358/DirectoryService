using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.ResultPattern;

namespace DirectoryServices.Presenters.ResponseExtensions
{
    public static class ResponseExtensions
    {
        public static ActionResult ToErrorResponse(this Error error)
        {
            return new ObjectResult(error)
            {
                StatusCode = GetStatusCodeFromErrorType(error.Type),
            };
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
