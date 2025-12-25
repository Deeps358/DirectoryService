using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirectoryServices.Contracts.Departaments
{
    public record GetChildrensByIdDto(List<DepartamentDto> Childrens, int ChildrensCount);
}