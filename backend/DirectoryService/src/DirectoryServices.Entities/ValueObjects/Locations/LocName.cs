using Shared.ResultPattern;

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
            return new LocName(name);
        }
    }
}
