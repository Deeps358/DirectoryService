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

                _logger.LogInformation("В базу добавлено новое подразделение с Id = {addedDepartament.Entity.Id.Value}", addedDepartament.Entity.Id.Value);
                return departament.Id.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("departament.incorrect.DB", ["Ошибка добавления подразделения в базу"]);
            }
        }

        public async Task<Result<Departament[]>> GetByIdAsync(Guid[] ids, CancellationToken cancellationToken)
        {
            try
            {
                DepId[] depIds = ids
                    .Select(DepId.GetCurrent)
                    .ToArray();

                Departament[] receivedDepartaments = await _dbContext.Departaments
                    .Where(d => depIds.Contains(d.Id))
                    .ToArrayAsync(cancellationToken);

                Guid[] missingIds = ids
                    .Except(receivedDepartaments
                    .Select(d => d.Id.Value))
                    .ToArray();

                if (missingIds.Length > 0) // нашли не всё что дали на вход
                {
                    string stringMissedIds = string.Join(", ", missingIds);
                    _logger.LogInformation("Департаменты с id = {missingIds} не найден!", stringMissedIds);
                    return Error.NotFound("departament.not_found.ids", [$"Департаменты с id = {stringMissedIds} не найдены!"]);
                }

                string stringIds = string.Join(", ", ids);

                _logger.LogInformation("Получены подразделения с id = {stringIds}", stringIds);

                return receivedDepartaments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении депов по id из БД");

                return Error.Failure("departament.incorrect.DB", ["Ошибка при получении депов из базы"]);
            }
        }

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> DeleteLocationsByDepAsync(DepId depId, CancellationToken cancellationToken)
        {
            int delAs = await _dbContext.DepartmentLocations
                .Where(dl => dl.DepartamentId == depId)
                .ExecuteDeleteAsync(cancellationToken);

            return CSharpFunctionalExtensions.UnitResult.Success<Error>();
        }
    }
}
