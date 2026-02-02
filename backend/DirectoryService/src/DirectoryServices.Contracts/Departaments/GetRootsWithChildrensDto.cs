namespace DirectoryServices.Contracts.Departaments
{
    public record GetRootsWithChildrensDto(List<DepartamentDto> Roots, int RootsCount);
}