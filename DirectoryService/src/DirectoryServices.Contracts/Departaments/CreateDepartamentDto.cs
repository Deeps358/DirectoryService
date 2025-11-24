namespace DirectoryServices.Contracts.Departaments
{
    public record CreateDepartamentDto(
        string Name,
        string Identifier,
        Guid? ParentId,
        Guid[] LocationsIds,
        bool IsActive);
}
