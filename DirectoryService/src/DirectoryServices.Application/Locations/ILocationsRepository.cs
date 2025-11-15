using DirectoryServices.Entities;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Locations
{
    public interface ILocationsRepository
    {
        Task<Result<Guid>> CreateAsync(Location location, CancellationToken cancellationToken);

        Task<Result<Location>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
