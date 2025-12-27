using DirectoryServices.Application.Departaments;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Departaments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("В базу добавлено новое подразделение с Id = {addedDepartament.Entity.Id.Value}", addedDepartament.Entity.Id.Value);

                return departament.Id.Value;
            }
            catch (DbUpdateException dbex)
            {
                var pgEx = dbex.InnerException as PostgresException;
                if (pgEx?.SqlState == "23505")
                {
                    _logger.LogInformation(pgEx.ConstraintName, "Сработал индекс уникальности при создании локации в БД");
                    return pgEx.ConstraintName switch
                    {
                        "IX_departaments_name" => Error.Conflict("departament.duplicate.name", ["В системе уже есть подразделение с таким именем!"]),
                        "IX_departaments_identifier" => Error.Conflict("departament.duplicate.identifier", ["В системе уже есть подразделение с таким идентификатором!"]),
                        _ => Error.Conflict("departament.duplicate.field", ["В системе уже есть подразделение с таким *ОШИБКА*"])
                    };
                }

                return Error.Failure("location.incorrect.DB", ["Ошибка добавления позиции в базу"]);
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
                    _logger.LogInformation("Департаменты с id = {stringMissedIds} не найден!", stringMissedIds);
                    return Error.NotFound("departament.not_found.ids", [$"Департаменты с id = {stringMissedIds} не найдены!"]);
                }

                string stringIds = string.Join(", ", ids);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Получены подразделения с id = {stringIds}", stringIds);

                return receivedDepartaments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении депов по id из БД");

                return Error.Failure("departament.incorrect.DB", ["Ошибка при получении депов из базы"]);
            }
        }

        public async Task<Result<Departament>> GetByIdWithLockAsync(Guid id, CancellationToken cancellationToken)
        {
            Departament? dep = await _dbContext.Departaments
                .FromSql($"SELECT * FROM departaments WHERE id = {id} AND is_active FOR UPDATE")
                .FirstOrDefaultAsync(cancellationToken);

            if (dep == null)
            {
                _logger.LogInformation("Департамент с id_with_lock = {id} не найден!", id);
                return Error.NotFound("departament.not_found.id_with_lock", [$"Департамент с id = {id} не найден!"]);
            }

            return dep;
        }

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> DeleteLocationsByDepAsync(DepId depId, CancellationToken cancellationToken)
        {
            int delAs = await _dbContext.DepartmentLocations
                .Where(dl => dl.DepartamentId == depId)
                .ExecuteDeleteAsync(cancellationToken);

            return CSharpFunctionalExtensions.UnitResult.Success<Error>();
        }

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> AddDepLocationsRelationsAsync(
            List<DepartmentLocation> deplocs,
            CancellationToken cancellationToken)
        {
            await _dbContext.DepartmentLocations.AddRangeAsync(deplocs, cancellationToken);
            return CSharpFunctionalExtensions.UnitResult.Success<Error>();
        }

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> GetChildDepsWithLockAsync(DepPath depPath, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                SELECT * FROM departaments WHERE path <@ {depPath.Value}::ltree ORDER BY depth FOR UPDATE 
                """;

                Departament[] childDeps = await _dbContext.Departaments.FromSql(sql).ToArrayAsync(cancellationToken);

                _logger.LogInformation("Получены дочерние подразделения с путём = {depPath.Value}", depPath.Value);

                return CSharpFunctionalExtensions.UnitResult.Success<Error>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении дочерних депов с путём = {depPath.Value}", depPath.Value);

                return Error.Failure("departament.DB.childrens", ["Ошибка при получении дочерних депов из базы"]);
            }
        }

        public async Task<Result<int>> MoveDepWithChildernsAsync(DepPath depPath, DepPath curParentPath, DepPath? newPath, DepId? parentId, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                UPDATE departaments
                SET path = 
                        {newPath?.Value ?? string.Empty}::ltree || subpath(path, nlevel({curParentPath.Value}::ltree)),
                    depth = 
                        nlevel({newPath?.Value ?? string.Empty}::ltree || subpath(path, nlevel({curParentPath.Value}::ltree))) - 1,
                    parent_id = 
                        CASE
                            WHEN path = {depPath.Value}::ltree
                            THEN {parentId?.Value}
                            ELSE parent_id
                        END
                WHERE path <@ {depPath.Value}::ltree;
                """;

                int affectedRows = await _dbContext.Database.ExecuteSqlAsync(sql);

                return affectedRows;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка при перемещении депов");
                return Error.Failure("departament.DB.move", ["Ошибка в базе при перемещении подразделений"]);
            }
        }

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> SoftDeleteDepartament(Guid depId, CancellationToken cancellationToken)
        {
            try
            {
                return CSharpFunctionalExtensions.UnitResult.Success<Error>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при мягком удалении подразделений в БД");

                return Error.Failure("departament.incorrect.softdelete", ["Ошибка при мягком удалении подразделений в базе"]);
            }
        }
    }
}
