using DirectoryServices.Application.Positions;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Npgsql;
using Shared.ResultPattern;

namespace DirectoryServices.Infrastructure.Postgres.Repositories
{
    public class PositionsRepository : IPositionsRepository
    {
        private readonly DirectoryServiceDbContext _dbContext;
        private readonly ILogger<PositionsRepository> _logger;

        public PositionsRepository(
            DirectoryServiceDbContext dbContext,
            ILogger<PositionsRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<Guid>> CreateAsync(Position position, CancellationToken cancellationToken)
        {
            try
            {
                var addedPosition = await _dbContext.Positions.AddAsync(position, cancellationToken);

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("В базу добавлена новая позиция с Id = {addedPosition.Entity.Id.Value}", addedPosition.Entity.Id.Value);
                return position.Id.Value;
            }
            catch (DbUpdateException dbex)
            {
                var pgEx = dbex.InnerException as PostgresException;
                if (pgEx?.SqlState == "23505")
                {
                    _logger.LogInformation(pgEx.ConstraintName, "Сработал индекс уникальности при создании позиции в БД");
                    return pgEx.ConstraintName switch
                    {
                        "IX_positions_name" => Error.Conflict("positions.duplicate.name", ["В системе уже есть позиция с таким именем!"]),
                        _ => Error.Conflict("positions.duplicate.field", ["В системе уже есть позиция с таким *ОШИБКА*"])
                    };
                }

                return Error.Failure("position.incorrect.DB", ["Ошибка добавления позиции в базу"]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("position.incorrect.unknown", ["Неизвестная ошибка добавления позиции в базу"]);
            }
        }

        public async Task<Result<Position[]>> GetByNameAsync(PosName[] names, CancellationToken cancellationToken)
        {
            try
            {
                Position[] receivedPositions = await _dbContext.Positions
                    .Where(p => names.Contains(p.Name))
                    .ToArrayAsync(cancellationToken);

                string[] missingNames = names
                    .Except(receivedPositions.Select(x => x.Name))
                    .Select(x => x.Value)
                    .ToArray();

                if (missingNames.Length > 0)
                {
                    string stringMissedNames = string.Join(", ", missingNames);
                    _logger.LogInformation("Не найдены позиции с именами: {stringNames}", stringMissedNames);
                    return Error.NotFound("positions.not_found.names", [$"Позиции с именами: {stringMissedNames} не найдены!"]);
                }

                string stringNames = string.Join(", ", names.Select(x => x.Value));

                _logger.LogInformation("Получены позиции с с именами: {stringNames}", stringNames);
                return receivedPositions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении позиций по имени из БД");
                return Error.Failure("positions.incorrect.DB", ["Ошибка при получении позиций по имени из базы"]);
            }
        }
    }
}
