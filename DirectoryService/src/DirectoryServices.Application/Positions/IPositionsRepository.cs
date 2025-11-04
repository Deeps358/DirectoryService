using DirectoryServices.Entities;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Positions
{
    public interface IPositionsRepository
    {
        Task<Result<Guid>> CreateAsync(Position position, CancellationToken cancellationToken);
    }
}
