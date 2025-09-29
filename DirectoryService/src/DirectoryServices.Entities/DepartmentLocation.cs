using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public class DepartmentLocation
    {
        private DepartmentLocation(
            Guid depId,
            Guid locId)
        {
            DepartamentId = depId;
            LocationId = locId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid DepartamentId { get; private set; }

        public Guid LocationId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<DepartmentLocation> Create(
            Guid depId,
            Guid locId)
        {
            var depLoc = new DepartmentLocation(depId, locId);

            return Result.Success(depLoc);
        }
    }
}
