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
            return new LocTimezone(timezone);
        }
    }
}
