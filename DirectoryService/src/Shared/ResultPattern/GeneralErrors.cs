namespace Shared.ResultPattern
{
    public static class GeneralErrors
    {
        public static Error IncorrectNameError(string entity)
            => Error.Validation($"{entity.ToLower()}.incorrect.name", "Название должно быть 3-150 символов!");
    }
}
