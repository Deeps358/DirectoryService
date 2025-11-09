using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryServices.Contracts.Departaments
{
    public record CreateDepartamentDto(
        string Name,
        string Identifier,
        Guid? ParentId,
        Guid[]? LocationsIds);
}
