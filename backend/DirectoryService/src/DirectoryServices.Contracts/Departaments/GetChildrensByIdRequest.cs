namespace DirectoryServices.Contracts.Departaments
{
    public record GetChildrensByIdRequest(
        int? Page,
        int? Size);
}