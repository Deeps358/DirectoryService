using System.Text.Json;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        protected BadRequestException(Error[] errors)
            : base(JsonSerializer.Serialize(errors))
        {
        }
    }
}
