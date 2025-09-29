using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public partial class Location
    {
        public record LocAdress
        {
            private LocAdress(string[] value)
            {
                Value = value;
            }

            public string[] Value { get; }

            public static Result<LocAdress> Create(string[] name)
            {
                // валидация имени
                if (!name.Any())
                {
                    return Result.Failure<LocAdress>("В адресе надо хоть что-то указать!");
                }

                return new LocAdress(name);
            }
        }
    }
}
