using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.Commands.ChangeParent
{
    public record ChangeParentCommand(Guid DepId, ChangeParentDto NewParent) : ICommand;
}