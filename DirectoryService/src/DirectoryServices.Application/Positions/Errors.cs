using Shared.ResultPattern;

namespace DirectoryServices.Application.Positions
{
    public partial class Errors
    {
        public static class Positions
        {
            public static Error TooLongDescription()
                => Error.Failure("loc.desc.count", "Слишком длинноео писание (более 1000 символов)");
        }
    }
}
