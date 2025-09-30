using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public class DepartmentPosition
    {
        private DepartmentPosition(
            Guid depId,
            Guid posId)
        {
            DepartamentId = depId;
            PositionId = posId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid DepartamentId { get; private set; }

        public Guid PositionId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<DepartmentPosition> Create(
            Guid depId,
            Guid posId)
        {
            var depPos = new DepartmentPosition(depId, posId);

            return Result.Success(depPos);
        }
    }
}
