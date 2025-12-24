namespace DirectoryServices.Contracts.Departaments
{
    public record GetRootsRequest(
        int? Page,
        int? Size,
        int? Prefetch);
}