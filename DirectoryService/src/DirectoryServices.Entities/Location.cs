using DirectoryServices.Entities.ValueObjects.Locations;
using Shared.ResultPattern;

namespace DirectoryServices.Entities
{
    public partial class Location
    {
        private readonly List<DepartmentLocation> _departmentLocations = [];

        public Location()
        {
            // efcore не ругайся
        }

        private Location(
            LocId id,
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

        public LocId Id { get; private set; } = null!;

        public LocName Name { get; private set; } = null!;

        public LocAdress Adress { get; private set; } = null!;

        public LocTimezone Timezone { get; private set; } = null!;

        public bool IsActive { get; private set; }

        public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public static Result<Location> Create(
            LocName name,
            LocAdress adress,
            LocTimezone timezone,
            bool isActive)
        {
            var locId = LocId.NewLocId();
            var location = new Location(locId, name, adress, timezone, isActive);

            return location;
        }
    }
}
