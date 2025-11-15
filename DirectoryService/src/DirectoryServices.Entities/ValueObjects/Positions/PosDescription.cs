using Shared.ResultPattern;

namespace DirectoryServices.Entities.ValueObjects.Positions
{
    public record PosDescription
    {
        public PosDescription()
        {
            // чтоб ефкор не ругался
        }

        private PosDescription(string? value)
        {
            Value = value;
        }

        public string? Value { get; } = null!;

        public static Result<PosDescription> Create(string? description)
        {
            return new PosDescription(description);
        }
    }
}
