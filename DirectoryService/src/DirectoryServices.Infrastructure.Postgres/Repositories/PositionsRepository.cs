using DirectoryServices.Application.Positions;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
                foreach (DepartmentPosition pos in position.DepartmentPositions)
                {
                    var addedRelation = await _dbContext.DepartmentPositions.AddAsync(pos, cancellationToken);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("В базу добавлена новая позиция с Id = {addedPosition.Entity.Id.Value}", addedPosition.Entity.Id.Value);
                return position.Id.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("position.incorrect.DB", ["Ошибка добавления позиции в базу"]);
            }
        }

        public async Task<Result<Position>> GetByNameAsync(PosName name, CancellationToken cancellationToken)
        {
            try
            {
                Position? receivedPosition = await _dbContext.Positions.FirstOrDefaultAsync(p => p.Name.Value == name.Value && p.IsActive == true, cancellationToken);
                if (receivedPosition != null)
                {
                    _logger.LogInformation("Получена позиция с именем {name.Value}", name.Value);
                }
                else
                {
                    _logger.LogInformation("Не найдена позиция с именем {name.Value}", name.Value);
                }

                return receivedPosition;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении позиции по имени из БД");
                return Error.Failure("departament.incorrect.DB", ["Ошибка при получении позиции из базы"]);
            }
        }
    }
}
