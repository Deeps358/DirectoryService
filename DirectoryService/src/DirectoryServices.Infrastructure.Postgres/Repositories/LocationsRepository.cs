using DirectoryServices.Application.Locations;
using DirectoryServices.Entities;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

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

        public async Task<Result<Guid>> CreateAsync(Location location, CancellationToken cancellationToken)
        {
            try
            {
                var addedLocation = await _dbContext.Locations.AddAsync(location, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("В базу добавлена новая локация с Id = {addedLocation.Entity.Id.Value}", addedLocation.Entity.Id.Value);

                return Result<Guid>.Success(location.Id.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("location.incorrect.DB", ["Ошибка добавления локации в базу"]);
            }
        }
    }
}
