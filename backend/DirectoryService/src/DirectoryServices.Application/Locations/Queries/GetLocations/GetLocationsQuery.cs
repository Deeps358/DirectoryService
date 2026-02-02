using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Locations;

namespace DirectoryServices.Application.Locations.Queries.GetLocations
{
    public record GetLocationQuery(GetLocationsRequest Request) : IQuery;
}