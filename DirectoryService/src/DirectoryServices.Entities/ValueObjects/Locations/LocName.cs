using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities.ValueObjects.Locations
{
    public record LocName
    {
        public LocName()
        {
            // чтоб ефкор не ругался
        }

        private LocName(string value)
        {
            Value = value;
        }

        public string Value { get; } = null!;

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
