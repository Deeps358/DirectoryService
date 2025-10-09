using DirectoryServices.Entities;
using DirectoryServices.Entities.Shared;

namespace DirectoryServices.Application.Locations
{
    public interface ILocationsRepository
    {
        Task<Result<Location>> CreateAsync(Location location, CancellationToken cancellationToken);
    }
}
