namespace DirectoryServices.Contracts.Locations
{
    public record CreateLocationDto(
        string Name,
        AdressDto Adress,
        string Timezone,
        bool isActive);
}
