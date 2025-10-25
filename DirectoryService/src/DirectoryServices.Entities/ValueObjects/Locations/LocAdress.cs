using Shared.ResultPattern;

namespace DirectoryServices.Entities.ValueObjects.Locations
{
    public record LocAdress
    {
        public LocAdress()
        {
            // чтоб ефкор не ругался
        }

        private LocAdress(string city, string street, int building, string room)
        {
            City = city;
            Street = street;
            Building = building;
            Room = room;
        }

        public string City { get; } = null!;

        public string Street { get; } = null!;

        public int Building { get; }

        public string Room { get; } = null!;

        public static Result<LocAdress> Create(string city, string street, int building, string room)
        {
            return new LocAdress(city, street, building, room);
        }
    }
}
