namespace DirectoryServices.Contracts.Locations
{
    public record CreateLocationRequest(
        string Name,
        AdressDto Adress,
        string Timezone,
        bool isActive);
}
