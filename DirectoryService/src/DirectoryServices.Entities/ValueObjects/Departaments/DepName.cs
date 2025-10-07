using CSharpFunctionalExtensions;

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
            // валидация имени
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3 || name.Length > 150)
            {
                return Result.Failure<DepName>("Название отдела должно быть 3-150 символов!");
            }

            return new DepName(name);
        }
    }
}
