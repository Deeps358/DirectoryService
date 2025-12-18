using DirectoryServices.Application.Abstractions;

namespace DirectoryServices.Application.Locations.Queries.GetLocationById
{
    public record GetLocationByIdQuery(Guid LocationId) : IQuery;
}