using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities.ValueObjects.Locations
{
    public record LocTimezone
    {
        public LocTimezone()
        {
            // чтоб ефкор не ругался
        }

        private LocTimezone(string value)
        {
            Value = value;
        }

        public string Value { get; } = null!;

        public static Result<LocTimezone> Create(string timezone)
        {
            // валидация зоны
            if (string.IsNullOrWhiteSpace(timezone))
            {
                return Result.Failure<LocTimezone>("Название временной зоны должно быть!");
            }

            try
            {
                var someTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            }
            catch (TimeZoneNotFoundException)
            {
                return Result.Failure<LocTimezone>("Название временной зоны введено некорректно!");
            }

            return new LocTimezone(timezone);
        }
    }
}
