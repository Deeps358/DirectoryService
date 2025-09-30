using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public partial class Position
    {
        public record PosName
        {
            private PosName(string value)
            {
                Value = value;
            }

            public string Value { get; }

            public static Result<PosName> Create(string name)
            {
                // валидация имени
                if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 150)
                {
                    return Result.Failure<PosName>("Название отдела должно быть 3-150 символов!");
                }

                return new PosName(name);
            }
        }
    }
}
