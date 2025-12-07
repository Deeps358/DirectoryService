using CSharpFunctionalExtensions;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Database;

public interface ITransactionManager
{
    Task<Shared.ResultPattern.Result<ITransactionScope>> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken);
}
