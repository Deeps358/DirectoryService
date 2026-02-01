using Shared.ResultPattern;

namespace DirectoryServices.Entities.ValueObjects.Departaments
{
    public record DepPath
    {
        public DepPath()
        {
            // чтоб ефкор не ругался
        }

        private DepPath(string value)
        {
            Value = value;
        }

        public string Value { get; } = null!;

        public static Result<DepPath> Create(string? depPath, DepIdentifier identifier)
        {
            // логика формирования пути отдела
            string path =
                depPath != null
                ? $"{depPath}.{identifier.Value}"
                : $"{identifier.Value}";

            return new DepPath(path);
        }

        public static Result<DepPath> GetCurrent(string depPath)
        {
            return new DepPath(depPath);
        }

        public static Result<DepPath> GetSoftDeleted(string depPath)
        {
            string deleteMark = "__DELETED__";
            return new DepPath(depPath + deleteMark);
        }
    }
}
