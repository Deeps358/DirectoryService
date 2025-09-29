using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public partial class Position
    {
        public record PosDescription
        {
            private PosDescription(string value)
            {
                Value = value;
            }

            public string Value { get; }

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
}
