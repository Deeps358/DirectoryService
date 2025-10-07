using CSharpFunctionalExtensions;
using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;

namespace DirectoryServices.Entities
{
    public class DepartmentLocation
    {
        public DepartmentLocation()
        {
            // чтоб ефкор не ругался
        }

        private DepartmentLocation(
            Guid id,
            DepId depId,
            LocId locId)
        {
            Id = id;
            DepartamentId = depId;
            LocationId = locId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public DepId DepartamentId { get; private set; } = null!;

        public LocId LocationId { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<DepartmentLocation> Create(
            DepId depId,
            LocId locId)
        {
            var depLoc = new DepartmentLocation(Guid.NewGuid(), depId, locId);

            return Result.Success(depLoc);
        }
    }
}
