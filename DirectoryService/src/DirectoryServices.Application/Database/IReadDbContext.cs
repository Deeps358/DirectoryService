using DirectoryServices.Entities;

namespace DirectoryServices.Application.Database
{
    public interface IReadDbContext
    {
        IQueryable<Location> LocationsRead { get; }

        IQueryable<Departament> DepartamentsRead { get; }

        IQueryable<Position> PositionsRead { get; }

        IQueryable<DepartmentLocation> DepartmentLocationsRead { get; }

        IQueryable<DepartmentPosition> DepartmentPositionsRead { get; }
    }
}
