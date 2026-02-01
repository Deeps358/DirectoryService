using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.Commands.CreateDepartament
{
    public record CreateDepartamentCommand(CreateDepartamentDto Departament) : ICommand;
}
