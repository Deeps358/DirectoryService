using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Positions;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Positions
{
    public interface IPositionsRepository
    {
        Task<Result<Guid>> CreateAsync(Position position, CancellationToken cancellationToken);

        Task<Result<Position>> GetByNameAsync(PosName name, CancellationToken cancellationToken);
    }
}
