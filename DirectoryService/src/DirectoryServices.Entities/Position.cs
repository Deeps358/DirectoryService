using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public partial class Position
    {
        private Position(
            Guid id,
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

        public Guid Id { get; private set; }

        public PosName Name { get; private set; }

        public PosDescription? Description { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<Position> Create(PosName name, PosDescription? description, bool isActive)
        {
            var position = new Position(Guid.NewGuid(), name, description, isActive);

            return Result.Success(position);
        }
    }
}
