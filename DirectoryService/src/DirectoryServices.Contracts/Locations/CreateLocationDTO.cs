namespace DirectoryServices.Contracts.Locations
{
    public record CreateLocationDTO(
        string Name,
        AdressDTO Adress,
        string Timezone,
        bool isActive);
}
