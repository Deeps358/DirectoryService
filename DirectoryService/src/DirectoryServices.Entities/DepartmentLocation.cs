using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Locations;
using Shared.ResultPattern;

namespace DirectoryServices.Entities
{
    public class DepartmentLocation
    {
        public DepartmentLocation()
        {
            // чтоб ефкор не ругался
        }

        private DepartmentLocation(
            DepId depId,
            LocId locId)
        {
            DepartamentId = depId;
            LocationId = locId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public DepId DepartamentId { get; private set; } = null!;

        public LocId LocationId { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<DepartmentLocation> Create(
            DepId depId,
            LocId locId)
        {
            var depLoc = new DepartmentLocation(depId, locId);

            return depLoc;
        }
    }
}
