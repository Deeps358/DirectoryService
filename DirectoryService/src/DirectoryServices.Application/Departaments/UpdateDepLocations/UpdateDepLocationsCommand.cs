using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.UpdateDepLocations
{
    public record UpdateDepLocationsCommand(Guid DepId, UpdateDepLocationsDto Locations) : ICommand;
}