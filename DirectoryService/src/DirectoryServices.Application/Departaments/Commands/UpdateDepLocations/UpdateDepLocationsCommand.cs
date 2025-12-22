using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.Commands.UpdateDepLocations
{
    public record UpdateDepLocationsCommand(Guid DepId, UpdateDepLocationsDto Locations) : ICommand;
}