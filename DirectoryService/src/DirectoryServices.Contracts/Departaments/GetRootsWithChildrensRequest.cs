namespace DirectoryServices.Contracts.Departaments
{
    public record GetRootsWithChildrensRequest(
        int? Page,
        int? Size,
        int? Prefetch);
}