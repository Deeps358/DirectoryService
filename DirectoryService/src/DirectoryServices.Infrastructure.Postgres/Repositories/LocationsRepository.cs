using DirectoryServices.Application.Locations;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Locations;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Result<Location>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                Location? receivedLocation = await _dbContext.Locations.FirstOrDefaultAsync(l => l.Id == LocId.GetCurrent(id), cancellationToken);
                if (receivedLocation == null)
                {
                    _logger.LogInformation("Локация с id = {id} не найдена!", id);
                    return Error.NotFound("location.notfound.id", [$"Локация с id = {id} не найдена!"]);
                }

                _logger.LogInformation("Получена локация с id = {id}", id);
                return Result<Location>.Success(receivedLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении локации по id из БД");

                return Error.Failure("locations.incorrect.DB", ["Ошибка при получении локации из базы"]);
            }
        }
    }
}
