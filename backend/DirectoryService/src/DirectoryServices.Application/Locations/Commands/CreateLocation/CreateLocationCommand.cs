using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Locations;

namespace DirectoryServices.Application.Locations.Commands.CreateLocation
{
    public record CreateLocationCommand(CreateLocationRequest Location) : ICommand;
}
