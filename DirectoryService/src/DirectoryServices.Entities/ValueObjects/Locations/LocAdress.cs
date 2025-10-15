using DirectoryServices.Contracts.Locations;
using Shared.ResultPattern;

namespace DirectoryServices.Entities.ValueObjects.Locations
{
    public record LocAdress
    {
        public LocAdress()
        {
            // чтоб ефкор не ругался
        }

        private LocAdress(AdressDto adress)
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

        public static Result<LocAdress> Create(AdressDto adress)
        {
            // валидация имени
            if (string.IsNullOrWhiteSpace(adress.City))
            {
                return Error.Validation("location.incorrect.city", "Название города пустое");
            }

            if (string.IsNullOrWhiteSpace(adress.Street))
            {
                return Error.Validation("location.incorrect.street", "Название улицы пустое");
            }

            if (adress.Building <= 0)
            {
                return Error.Validation("location.incorrect.building", "Странный номер здания");
            }

            if (string.IsNullOrWhiteSpace(adress.Room))
            {
                return Error.Validation("location.incorrect.room", "Номер комнаты пуст");
            }

            return new LocAdress(adress);
        }
    }
}
