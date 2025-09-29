using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public partial class Departament
    {
        public record DepName
        {
            private DepName(string value)
            {
                Value = value;
            }

            public string Value { get; }

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
}
