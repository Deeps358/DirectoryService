using DirectoryServices.Entities.Shared;

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
            // валидация имени
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 150)
            {
                return "Название отдела должно быть 3-150 символов!";
            }

            return new PosName(name);
        }
    }
}
