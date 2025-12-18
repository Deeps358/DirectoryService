namespace DirectoryServices.Contracts.Locations
{
    public record GetLocationDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string City { get; init; } = string.Empty;

        public string Street { get; init; } = string.Empty;

        public int Building { get; init; }

        public string Room { get; init; } = string.Empty;

        public string Timezone { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public Guid[]? DepartmentLocations { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}