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

        public async Task<Result<Location[]>> GetByIdAsync(Guid[] ids, CancellationToken cancellationToken)
        {
            ids = ids.Distinct().ToArray();
            try
            {
                LocId[] locIds = ids
                    .Select(LocId.GetCurrent)
                    .ToArray();

                Location[] receivedLocation = await _dbContext.Locations
                    .Where(l => locIds.Contains(l.Id))
                    .ToArrayAsync(cancellationToken);

                Guid[] missingIds = ids
                    .Except(receivedLocation
                    .Select(x => x.Id.Value))
                    .ToArray();

                if (missingIds.Length > 0) // нашли не всё что дали на вход
                {
                    string stringMissedIds = string.Join(", ", missingIds);
                    _logger.LogInformation("Локации с id = {stringMissedIds} не найдены!", stringMissedIds);
                    return Error.NotFound("location.not_found.ids", [$"Локации с id = {stringMissedIds} не найдены!"]);
                }

                string stringIds = string.Join(", ", ids);

                _logger.LogInformation("Получены локации с id = {stringIds}", stringIds);
                return receivedLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении локаций по id из БД");

                return Error.Failure("locations.incorrect.DB", ["Ошибка при получении локаций из базы"]);
            }
        }
    }
}
