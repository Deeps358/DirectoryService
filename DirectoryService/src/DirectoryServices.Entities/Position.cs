using DirectoryServices.Entities.ValueObjects.Positions;
using Shared.ResultPattern;

namespace DirectoryServices.Entities
{
    public partial class Position
    {
        public Position()
        {
            // efcore не ругайся
        }

        private Position(
            PosId id,
            PosName name,
            PosDescription? description,
            bool isActive)
        {
            Id = id;
            Name = name;
            Description = description;
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public PosId Id { get; private set; } = null!;

        public PosName Name { get; private set; } = null!;

        public PosDescription? Description { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<Position> Create(PosName name, PosDescription? description, bool isActive)
        {
            var posId = PosId.NewPosId();
            var position = new Position(posId, name, description, isActive);

            return position;
        }
    }
}
