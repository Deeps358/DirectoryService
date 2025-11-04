using DirectoryServices.Application.Departaments;
using DirectoryServices.Entities;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Infrastructure.Postgres.Repositories
{
    public class DepartamentsRepository : IDepartamentsRepository
    {
        private readonly DirectoryServiceDbContext _dbContext;
        private readonly ILogger<DepartamentsRepository> _logger;

        public DepartamentsRepository(
            DirectoryServiceDbContext dbContext,
            ILogger<DepartamentsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<Guid>> CreateAsync(Departament departament, CancellationToken cancellationToken)
        {
            try
            {
                var addedDepartament = await _dbContext.Departaments.AddAsync(departament, cancellationToken);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("В базу добавлено новое подразделение с Id = {addedDepartament.Entity.Id.Value}", addedDepartament.Entity.Id.Value);
                return Result<Guid>.Success(departament.Id.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("departament.incorrect.DB", ["Ошибка добавления подразделения в базу"]);
            }
        }
    }
}
