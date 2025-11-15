using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Departaments;

namespace DirectoryServices.Application.Departaments.CreateDepartament
{
    public record CreateDepartamentCommand(CreateDepartamentDto Departament) : ICommand;
}
