using DirectoryServices.Application.Abstractions;

namespace DirectoryServices.Application.Departaments.Commands.SoftDelete
{
    public record SoftDeleteCommand(Guid DepId) : ICommand;
}