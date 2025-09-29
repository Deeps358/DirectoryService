using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public partial class Location
    {
        public record LocName
        {
            private LocName(string value)
            {
                Value = value;
            }

            public string Value { get; }

            public static Result<LocName> Create(string name)
            {
                // валидация имени
                if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 120)
                {
                    return Result.Failure<LocName>("Название локации должно быть 3-120 символов!");
                }

                return new LocName(name);
            }
        }
    }
}
