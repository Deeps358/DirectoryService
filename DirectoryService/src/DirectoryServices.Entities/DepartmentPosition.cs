using DirectoryServices.Entities.ValueObjects.Departaments;
using DirectoryServices.Entities.ValueObjects.Positions;
using Shared.ResultPattern;

namespace DirectoryServices.Entities
{
    public class DepartmentPosition
    {
        public DepartmentPosition()
        {
            // чтоб ефкор не ругался
        }

        private DepartmentPosition(
            Guid id,
            DepId depId,
            PosId posId)
        {
            Id = id;
            DepartamentId = depId;
            PositionId = posId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public DepId DepartamentId { get; private set; } = null!;

        public PosId PositionId { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<DepartmentPosition> Create(
            DepId depId,
            PosId posId)
        {
            var depPos = new DepartmentPosition(Guid.NewGuid(), depId, posId);

            return depPos;
        }
    }
}
