using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Positions;

namespace DirectoryServices.Application.Positions.CreatePosition
{
    public record CreatePositionCommand(CreatePositionDto Position) : ICommand;
}
