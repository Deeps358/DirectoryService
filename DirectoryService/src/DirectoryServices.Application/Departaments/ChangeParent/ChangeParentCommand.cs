using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.ChangeParent
{
    public record ChangeParentCommand(Guid DepId, ChangeParentDto NewParent) : ICommand;
}