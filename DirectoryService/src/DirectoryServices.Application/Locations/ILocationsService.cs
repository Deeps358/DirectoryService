using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Locations
{
    public interface ILocationsService
    {
        Task<Result<Location>> Create(CreateLocationDto location, CancellationToken cancellationToken);
    }
}
