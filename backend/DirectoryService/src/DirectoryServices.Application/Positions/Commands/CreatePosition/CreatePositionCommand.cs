using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Positions;

namespace DirectoryServices.Application.Positions.Commands.CreatePosition
{
    public record CreatePositionCommand(CreatePositionDto Position) : ICommand;
}
