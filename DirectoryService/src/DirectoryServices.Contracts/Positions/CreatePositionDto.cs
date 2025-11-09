namespace DirectoryServices.Contracts.Positions
{
    public record CreatePositionDto(
        string Name,
        string Description,
        bool IsActive);
}
