using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Locations;

namespace DirectoryServices.Application.Locations.CreateLocation
{
    public record CreateLocationCommand(CreateLocationDto Location) : ICommand;
}
