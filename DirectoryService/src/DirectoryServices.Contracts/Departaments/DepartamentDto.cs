namespace DirectoryServices.Contracts.Departaments
{
    public record DepartamentDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public string Identifier { get; init; }

        public Guid? ParentId { get; init; }

        public string Path { get; init; }

        public short Depth { get; init; }

        public bool IsActive { get; init; }

        public bool HasMoreChildrens { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}