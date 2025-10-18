namespace Shared.ResultPattern
{
    public static class GeneralErrors
    {
        public static Error NotFound(Guid? id)
        {
            string[] message = id == null ? [$"Сущность не найдена"] : [$"Сущность с id {id} не найдена"];
            return Error.NotFound(null, message);
        }

        public static Error Validation(string message, string? invalidField = null)
            => Error.Validation(null, ["Некорректно заполненое поле"], invalidField);

        public static Error Conflict(string[] message)
            => Error.Conflict(null, message);

        public static Error Failure(string[] message)
            => Error.Failure(null, message);

        public static Error IncorrectNameError(string entity)
            => Error.Validation($"{entity.ToLower()}.incorrect.name", ["Название должно быть 3-150 символов!"]);
    }
}
