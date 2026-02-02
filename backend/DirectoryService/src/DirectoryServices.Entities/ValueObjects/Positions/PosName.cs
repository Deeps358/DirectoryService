using Shared.ResultPattern;

namespace DirectoryServices.Entities.ValueObjects.Positions
{
    public record PosName
    {
        public PosName()
        {
            // чтоб ефкор не ругался
        }

        private PosName(string value)
        {
            Value = value;
        }

        public string Value { get; } = null!;

        public static Result<PosName> Create(string name)
        {
            return new PosName(name);
        }
    }
}
