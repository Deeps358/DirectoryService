using Dapper;
using DirectoryServices.Application.Database;
using DirectoryServices.Application.Departaments;
using DirectoryServices.Contracts.Departaments;
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
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<DepartamentsRepository> _logger;

        public DepartamentsRepository(
            DirectoryServiceDbContext dbContext,
            IDbConnectionFactory connectionFactory,
            ILogger<DepartamentsRepository> logger)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
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

        public async Task<Result<Departament[]>> GetByIdsWithLockAsync(DepId[] ids, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                    SELECT *
                    FROM departaments d
                    WHERE d.id = ANY({ids.Select(i => i.Value).ToArray()}::uuid[]) FOR UPDATE
                    """;

                Departament[] lockedDeps = await _dbContext.Departaments.FromSql(sql).ToArrayAsync();

                if (lockedDeps == null || lockedDeps.Length != ids.Length)
                {
                    _logger.LogInformation("Не все депы из массива найдены!");
                    return Error.NotFound("departament.not_found.ids_with_lock", [$"Внутренняя ошибка БД, попробуйте снова"]);
                }

                return lockedDeps;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при блокировке массива депов");

                return Error.Failure("departament.DB.ids_with_lock", ["Ошибка при блокировке массива депов"]);
            }
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

        public async Task<Result<int>> LockDepsAndChildsAsync(DepPath[] depPaths, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                SELECT *
                FROM departaments d
                WHERE d.path <@ ANY({depPaths.Select(dp => dp.Value).ToArray()}::ltree[]) FOR UPDATE 
                """;

                var lockedDeps = await _dbContext.Departaments.FromSql(sql).ToArrayAsync();

                if (lockedDeps == null)
                {
                    _logger.LogError("Ошибка при блокировке депов с детьми, их 0");
                    return Error.Failure("departament.DB.childrens.zero", ["Ошибка при блокировке депов с детьми"]);
                }

                _logger.LogInformation("Выполнена блокировка {lockedDeps.Length} подразделений", lockedDeps.Length);

                return lockedDeps.Length;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при блокировке депов с детьми");

                return Error.Failure("departament.DB.childrens", ["Ошибка при блокировке депов с детьми"]);
            }
        }

        public async Task<Result<int>> MoveDepWithChildernsAsync(DepPath depPath, DepPath curParentPath, DepPath? newParentPath, DepId? parentId, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                UPDATE departaments
                SET path = 
                        {newParentPath?.Value ?? string.Empty}::ltree || subpath(path, nlevel({curParentPath.Value}::ltree)),
                    depth = 
                        nlevel({newParentPath?.Value ?? string.Empty}::ltree || subpath(path, nlevel({curParentPath.Value}::ltree))) - 1,
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

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> SoftDeleteWithChildrensAsync(DepPath oldPath, DepPath deletedPath, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                UPDATE departaments
                SET path = 
                    CASE
                        WHEN path = {oldPath.Value}::ltree
                        THEN {deletedPath.Value}::ltree
                        ELSE {deletedPath.Value}::ltree || subpath(path, nlevel({oldPath.Value}::ltree))
                    END,
                    is_active = 
                    CASE
                        WHEN path = {oldPath.Value}::ltree
                        THEN false
                        ELSE is_active
                    END,
                    deleted_at = 
                    CASE
                        WHEN path = {oldPath.Value}::ltree
                        THEN {DateTime.UtcNow}
                        ELSE deleted_at
                    END
                WHERE path <@ {oldPath.Value}::ltree;
                """;

                await _dbContext.Database.ExecuteSqlAsync(sql);

                _logger.LogInformation("Мягкое удаление депа с путём = {oldPath.Value}", oldPath.Value);

                return CSharpFunctionalExtensions.UnitResult.Success<Error>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при мягком удалении подразделения с путём {oldPath.Value} в БД", oldPath.Value);

                return Error.Failure("departament.incorrect.softdelete", ["Ошибка при мягком удалении подразделений в базе"]);
            }
        }

        public async Task<Result<int>> DeactivateLocationsWithDepId(Guid depId, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                UPDATE locations l
                SET 
                is_active = false,
                deleted_at = {DateTime.UtcNow}
                WHERE l.id IN (
                    SELECT dl1.location_id
                    FROM departament_locations dl1
                    WHERE dl1.departament_id = {depId}
                    AND NOT EXISTS (
                        SELECT 1
                        FROM departament_locations dl2
                        WHERE dl2.location_id = dl1.location_id
                        AND dl2.departament_id != {depId}
                    )
                )
                """;

                var locsCount = await _dbContext.Database.ExecuteSqlAsync(sql);

                _logger.LogInformation("{locsCount} локации стали неактивными после SoftDelete депа с id = {depId}", locsCount, depId);

                return locsCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка деактивации локаций при мягком удалении подразделения с id = {depId} в БД", depId);

                return Error.Failure("departament.incorrect.softdelete.locations", ["Ошибка деактивации локаций при мягком удалении подразделения в базе"]);
            }
        }

        public async Task<Result<int>> DeactivatePositionsWithDepId(Guid depId, CancellationToken cancellationToken)
        {
            try
            {
                FormattableString sql = $"""
                UPDATE positions p
                SET 
                is_active = false,
                deleted_at = {DateTime.UtcNow}
                WHERE p.id IN (
                    SELECT dp1.position_id
                    FROM departament_positions dp1
                    WHERE dp1.departament_id = {depId}
                    AND NOT EXISTS (
                        SELECT 1
                        FROM departament_positions dp2
                        WHERE dp2.position_id = dp1.position_id
                        AND dp2.departament_id != {depId}
                    )
                )
                """;

                var posCount = await _dbContext.Database.ExecuteSqlAsync(sql);

                _logger.LogInformation("{posCount} позиций стали неактивными после SoftDelete депа с id = {depId}", posCount, depId);

                return posCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка деактивации позиций при мягком удалении подразделения с id = {depId} в БД", depId);

                return Error.Failure("departament.incorrect.softdelete.positions", ["Ошибка деактивации позиций при мягком удалении подразделения в базе"]);
            }
        }

        public async Task<Departament[]?> GetDepsChildrensFirstLevelById(DepId[] depIds, CancellationToken cancellationToken)
        {
            Departament[]? childs = await _dbContext.Departaments.Where(d => depIds.Contains(d.ParentId)).ToArrayAsync();
            if (childs == null)
            {
                _logger.LogInformation("Запрос для взятия детей депа ничего не нашёл");
            }

            return childs;
        }

        public async Task<Departament[]?> GetDepsForHardDelete(CancellationToken cancellationToken)
        {
            Departament[]? oldDeps = await _dbContext.Departaments.Where(d => d.DeletedAt.Value.AddMonths(1) < DateTime.UtcNow).ToArrayAsync();

            if (oldDeps == null)
            {
                _logger.LogInformation("Запрос для поиска старых депов ничего не нашёл");
            }

            return oldDeps;
        }

        public async Task<CSharpFunctionalExtensions.UnitResult<Error>> HardDeleteDep(DepId[] ids, CancellationToken cancellationToken)
        {
            try
            {
                int delDeps = await _dbContext.Departaments.Where(d => ids.Contains(d.Id)).ExecuteDeleteAsync();

                _logger.LogInformation("Полностью удалено {delDeps} подразделений", delDeps);

                return CSharpFunctionalExtensions.UnitResult.Success<Error>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при жёстком удалении депов из базы");
                return Error.Failure("departament.incorrect.harddelete.departaments", ["Ошибка жёсткого удаления подразделений в базе"]);
            }
        }
    }
}
