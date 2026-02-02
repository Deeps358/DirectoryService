using Shared.ResultPattern;

namespace DirectoryServices.Entities.ValueObjects.Departaments
{
    public record DepName
    {
        public DepName()
        {
            // чтоб ефкор не ругался
        }

        private DepName(string value)
        {
            Value = value;
        }

        public string Value { get; } = null!;

        public static Result<DepName> Create(string name)
        {
            return new DepName(name);
        }
    }
}
