using DirectoryServices.Contracts.Locations;
using Microsoft.AspNetCore.Mvc.Formatters;
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
            List<string> errs = new();

            // валидация
            if (string.IsNullOrWhiteSpace(adress.City))
            {
                errs.Add("Название города пустое");
            }

            if (string.IsNullOrWhiteSpace(adress.Street))
            {
                errs.Add("Название улицы пустое");
            }

            if (adress.Building <= 0)
            {
                errs.Add("Странный номер здания");
            }

            if (string.IsNullOrWhiteSpace(adress.Room))
            {
                errs.Add("Номер комнаты пуст");
            }

            if(errs.Any())
            {
                return Error.Validation("location.incorrect.adress", errs.ToArray());
            }

            return new LocAdress(adress);
        }
    }
}
