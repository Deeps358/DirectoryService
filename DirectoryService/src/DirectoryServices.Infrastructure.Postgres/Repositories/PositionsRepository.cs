using DirectoryServices.Application.Positions;
using DirectoryServices.Entities;
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
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("В базу добавлена новая позиция с Id = {addedPosition.Entity.Id.Value}", addedPosition.Entity.Id.Value);
                return Result<Guid>.Success(position.Id.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи в БД");

                return Error.Failure("position.incorrect.DB", ["Ошибка добавления позиции в базу"]);
            }
        }
    }
}
