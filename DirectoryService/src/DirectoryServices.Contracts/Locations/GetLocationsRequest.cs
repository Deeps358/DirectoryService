namespace DirectoryServices.Contracts.Locations
{
    public record GetManyLocsRequest(GetLocationsRequest r);
    public record GetLocationsRequest(
        Guid[]? DepartamentIds,
        string? Search,
        bool? IsActive,
        string? SortBy,
        int? Page,
        int? PageSize);
}