namespace DirectoryServices.Contracts.Locations
{
    public record GetLocationsDto(List<LocationDto> Locations, long TotaCount);

    public record LocationDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public AdressDto Adress { get; init; }

        public string Timezone { get; init; } = string.Empty;

        public bool IsActive { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}