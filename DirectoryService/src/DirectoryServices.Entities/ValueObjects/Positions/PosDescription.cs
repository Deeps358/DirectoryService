using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities.ValueObjects.Positions
{
    public record PosDescription
    {
        public PosDescription()
        {
            // чтоб ефкор не ругался
        }

        private PosDescription(string value)
        {
            Value = value;
        }

        public string Value { get; } = null!;

        public static Result<PosDescription> Create(string? description)
        {
            if (description == null)
            {
                return null;
            }

            // валидация описания
            if (description.Length > 1000)
            {
                return Result.Failure<PosDescription>("Описание позиции должно быть 1-1000 символов!");
            }

            return new PosDescription(description);
        }
    }
}
