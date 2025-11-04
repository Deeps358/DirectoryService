using DirectoryServices.Entities;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Locations
{
    public interface ILocationsRepository
    {
        Task<Result<Guid>> CreateAsync(Location location, CancellationToken cancellationToken);
    }
}
