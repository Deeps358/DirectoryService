using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
using DirectoryServices.Entities.Shared;

namespace DirectoryServices.Application.Locations
{
    public interface ILocationsService
    {
        Task<Result<Location>> Create(CreateLocationDTO location, CancellationToken cancellationToken);
    }
}
