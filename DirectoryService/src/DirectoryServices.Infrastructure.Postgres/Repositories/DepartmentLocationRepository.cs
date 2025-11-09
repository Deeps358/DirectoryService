using DirectoryServices.Application;
using DirectoryServices.Entities;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Infrastructure.Postgres.Repositories
{
    public class DepartmentLocationRepository : IDepartmentLocationRepository
    {
        private readonly DirectoryServiceDbContext _dbContext;
        private readonly ILogger<DepartmentLocationRepository> _logger;

        public DepartmentLocationRepository(DirectoryServiceDbContext dbContext, ILogger<DepartmentLocationRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<Guid>> CreateAsync(DepartmentLocation deploc, CancellationToken cancellationToken)
        {
            try
            {
                var addedDepLoc = await _dbContext.DepartmentLocations.AddAsync(deploc, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "В базу добавлена новая связь депа {addedDepLoc.Entity.DepartamentId.Value} с локацией {addedDepLoc.Entity.LocationId.Value}",
                    addedDepLoc.Entity.DepartamentId.Value,
                    addedDepLoc.Entity.LocationId.Value);

                return Result<Guid>.Success(addedDepLoc.Entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("deploc.incorrect.DB", ["Ошибка добавления связи подразделения с локацией в базу"]);
            }
        }
    }
}
