using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.Queries.GetRoots.GetChildrensById
{
    public record GetChildrensByIdQuery(GetChildrensByIdRequest Request, Guid ParentId) : IQuery;
}