using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{

    public partial class Departament
    {
        public record DepPath
        {
            private DepPath(string value)
            {
                Value = value;
            }

            public string Value { get; }

            public static Result<DepPath> Create(Departament dep, DepIdentifier identifier)
            {
                // логика формирования пути отдела
                string path =
                    dep != null
                    ? $"{dep.Path.Value}.{identifier.Value}"
                    : $"{identifier.Value}";

                return new DepPath(path);
            }
        }
    }
}
