using System.Text.RegularExpressions;
using Shared.ResultPattern;

namespace DirectoryServices.Entities.ValueObjects.Departaments
{
    public record DepIdentifier
    {
        private DepIdentifier()
        {
            // чтоб ефкор не ругался
        }

        private DepIdentifier(string value)
        {
            Value = value;
        }

        public string Value { get; } = null!;

        public static Result<DepIdentifier> Create(string identifier)
        {
            // валидация идентификатора
            if (string.IsNullOrWhiteSpace(identifier) || identifier.Length < 2 || identifier.Length > 10)
            {
                return Error.Validation("departament.incorrect.identifier", "Идентификатор отдела должно быть 2-10 символов!");
            }

            if (!Regex.IsMatch(identifier, @"^[a-z\-]+$"))
            {
                return Error.Validation("departament.incorrect.identifier", "В идентификаторе допускаются только латиница в нижнем регистре и дефисы");
            }

            return new DepIdentifier(identifier);
        }
    }
}
