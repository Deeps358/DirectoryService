using DirectoryServices.Entities.ValueObjects.Positions;
using Shared.ResultPattern;

namespace DirectoryServices.Entities
{
    public partial class Position
    {
        private readonly List<DepartmentPosition> _departmentPositions = [];

        public Position()
        {
            // efcore не ругайся
        }

        private Position(
            PosId id,
            PosName name,
            PosDescription? description,
            IEnumerable<DepartmentPosition> departments,
            bool isActive)
        {
            Id = id;
            Name = name;
            Description = description;
            _departmentPositions = departments.ToList();
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public PosId Id { get; private set; } = null!;

        public PosName Name { get; private set; } = null!;

        public PosDescription? Description { get; private set; }

        public bool IsActive { get; private set; }

        public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<Position> Create(
            PosId id,
            PosName name,
            PosDescription? description,
            IEnumerable<DepartmentPosition> departments,
            bool isActive)
        {
            var position = new Position(id, name, description, departments, isActive);

            return position;
        }
    }
}
