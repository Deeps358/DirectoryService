using DirectoryServices.Application.Departaments;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Microsoft.EntityFrameworkCore;
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
                var addedDepartament = await _dbContext.Departaments.AddAsync(departament, cancellationToken); // сохраняем деп
                foreach (DepartmentLocation dep in departament.DepartamentLocations)
                {
                    var addedRelation = await _dbContext.DepartmentLocations.AddAsync(dep, cancellationToken); // не забываем о связях
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("В базу добавлено новое подразделение с Id = {addedDepartament.Entity.Id.Value}", addedDepartament.Entity.Id.Value);
                return departament.Id.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("departament.incorrect.DB", ["Ошибка добавления подразделения в базу"]);
            }
        }

        public async Task<Result<Departament>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                Departament? receivedDepartament = await _dbContext.Departaments.FirstOrDefaultAsync(d => d.Id == DepId.GetCurrent(id), cancellationToken);
                if (receivedDepartament == null)
                {
                    _logger.LogInformation("Департамент с id = {id} не найден!", id);
                    return Error.NotFound("departament.not_found.id", [$"Департамент с id = {id} не найден!"]);
                }

                _logger.LogInformation("Получено подразделение с id = {id}", id);
                return receivedDepartament;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении депа по id из БД");

                return Error.Failure("departament.incorrect.DB", ["Ошибка при получении депа из базы"]);
            }
        }
    }
}
