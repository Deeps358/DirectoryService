using Shared.ResultPattern;

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
                return Error.Validation("location.empty.timezone", "Название временной зоны должно быть!");
            }

            try
            {
                var someTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            }
            catch (TimeZoneNotFoundException)
            {
                return Error.Validation("location.incorrect.timezone", "Название временной зоны введено некорректно!");
            }

            return new LocTimezone(timezone);
        }
    }
}
