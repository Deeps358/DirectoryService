using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.Queries.GetRoots
{
    public record GetRootsWithChildrensQuery(GetRootsWithChildrensRequest Request) : IQuery;
}