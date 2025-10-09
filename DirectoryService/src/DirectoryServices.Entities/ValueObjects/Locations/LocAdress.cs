using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities.Shared;

namespace DirectoryServices.Entities.ValueObjects.Locations
{
    public record LocAdress
    {
        public LocAdress()
        {
            // чтоб ефкор не ругался
        }

        private LocAdress(AdressDTO adress)
        {
            City = adress.City;
            Street = adress.Street;
            Building = adress.Building;
            Room = adress.Room;
        }

        public string City { get; } = null!;

        public string Street { get; } = null!;

        public int Building { get; }

        public string Room { get; } = null!;

        public static Result<LocAdress> Create(AdressDTO adress)
        {
            // валидация имени
            if (string.IsNullOrWhiteSpace(adress.City))
            {
                return "Название города пустое";
            }

            if (string.IsNullOrWhiteSpace(adress.Street))
            {
                return "Название улицы пустое";
            }

            if (adress.Building <= 0)
            {
                return "Странный номер здания";
            }

            if (string.IsNullOrWhiteSpace(adress.Room))
            {
                return "Номер комнаты пуст";
            }

            return new LocAdress(adress);
        }
    }
}
