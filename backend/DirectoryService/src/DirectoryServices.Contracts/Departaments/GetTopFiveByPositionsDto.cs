namespace DirectoryServices.Contracts.Departaments
{
    public record GetTopFiveByPositionsDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public int PositionsCount { get; init; }

        public bool IsActive { get; init; }
    }
}