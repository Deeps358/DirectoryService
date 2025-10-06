﻿using CSharpFunctionalExtensions;

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

        public static Result<DepPath> Create(string depPath, DepIdentifier identifier)
        {
            // логика формирования пути отдела
            string path =
                depPath != null
                ? $"{depPath}.{identifier.Value}"
                : $"{identifier.Value}";

            return new DepPath(path);
        }
    }
}
