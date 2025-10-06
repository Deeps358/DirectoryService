using CSharpFunctionalExtensions;

namespace DirectoryServices.Entities.ValueObjects.Locations
{
    public record LocAdress
    {
        public LocAdress()
        {
            // чтоб ефкор не ругался
        }

        private LocAdress(string city, string street)
        {
            City = city;
            Street = street;
        }

        public string City { get; } = null!;

        public string Street { get; } = null!;

        public static Result<LocAdress> Create(string city, string street)
        {
            // валидация имени
            if (string.IsNullOrWhiteSpace(city))
            {
                return Result.Failure<LocAdress>("Название города пустое");
            }

            if (string.IsNullOrWhiteSpace(street))
            {
                return Result.Failure<LocAdress>("Название улицы пустое");
            }

            return new LocAdress(city, street);
        }
    }
}
