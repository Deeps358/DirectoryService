using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.Queries.GetChildrensById
{
    public record GetChildrensByIdQuery(GetChildrensByIdRequest Request, Guid ParentId) : IQuery;
}