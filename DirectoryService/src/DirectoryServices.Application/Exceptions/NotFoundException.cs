using System.Text.Json;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        protected NotFoundException(Error[] errors)
            : base(JsonSerializer.Serialize(errors))
        {

        }
    }
}
