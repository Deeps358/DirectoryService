using System.Text.Json.Serialization;

namespace Shared.ResultPattern
{
    public record Error
    {
        public static Error None = new Error(string.Empty, string.Empty, ErrorType.NONE);

        public string Code { get; }

        public string Message { get; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ErrorType Type { get; }

        public string? InvalidField { get; }

        [JsonConstructor]
        private Error(
            string code,
            string message,
            ErrorType type,
            string? invalidField = null)
        {
            Code = code;
            Message = message;
            Type = type;
            InvalidField = invalidField;
        }

        public static Error NotFound(string? code, string message, Guid? id)
            => new(code ?? "record.not.found", message, ErrorType.NOT_FOUND);

        public static Error Validation(string? code, string message, string? invalidField = null)
            => new(code ?? "invalid.value", message, ErrorType.VALIDATION, invalidField);

        public static Error Conflict(string? code, string message)
            => new(code ?? "conflict.value", message, ErrorType.CONFLICT);

        public static Error Failure(string? code, string message)
            => new(code ?? "something.wrong", message, ErrorType.FAILURE);
    }

    public enum ErrorType
    {
        /// <summary>
        /// Неизвестно
        /// </summary>
        NONE,

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        VALIDATION,

        /// <summary>
        /// Объект не найден
        /// </summary>
        NOT_FOUND,

        /// <summary>
        /// Что-то упало
        /// </summary>
        FAILURE,

        /// <summary>
        /// Конфликт в БД
        /// </summary>
        CONFLICT,
    }
}
