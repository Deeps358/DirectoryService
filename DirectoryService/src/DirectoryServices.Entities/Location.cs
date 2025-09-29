using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities
{
    public partial class Location
    {
        private Location(
            Guid id,
            LocName name,
            LocAdress adress,
            LocTimezone timezone,
            bool isActive)
        {
            Id = id;
            Name = name;
            Adress = adress;
            Timezone = timezone;
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }

        public LocName Name { get; private set; }

        public LocAdress Adress { get; private set; }

        public LocTimezone Timezone { get; private set; }

        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<Location> Create(
            LocName name,
            LocAdress adress,
            LocTimezone timezone,
            bool isActive)
        {
            var location = new Location(Guid.NewGuid(), name, adress, timezone, isActive);

            return Result.Success(location);
        }
    }
}
