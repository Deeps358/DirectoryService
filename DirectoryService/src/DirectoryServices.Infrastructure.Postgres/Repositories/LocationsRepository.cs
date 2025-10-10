using DirectoryServices.Application.Locations;
using DirectoryServices.Entities;
using DirectoryServices.Entities.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryServices.Infrastructure.Postgres.Repositories
{
    public class LocationsRepository : ILocationsRepository
    {
        private readonly DirectoryServiceDbContext _dbContext;
        private readonly ILogger<LocationsRepository> _logger;

        public LocationsRepository(DirectoryServiceDbContext dbContext, ILogger<LocationsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<Location>> CreateAsync(Location location, CancellationToken cancellationToken)
        {
            try
            {
                var addedLocation = await _dbContext.Locations.AddAsync(location, cancellationToken);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Добавлена сущность локации с {addedLocation.Entity.Id.Value}", addedLocation.Entity.Id.Value);

                return Result<Location>.Success(location);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Ошибка при записи в БД: {ex.Message}");

                return "Ошибка записи сущности Location в базу";
            }
        }
    }
}
